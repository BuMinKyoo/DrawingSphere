using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

// 원본 예제처럼 광선과 충돌 결과를 작은 자료형으로 표현합니다.
struct Ray
{
    public Vector3 Start;
    public Vector3 Direction;
}

struct Hit
{
    public float Distance;
    public Vector3 Point;
    public Vector3 Normal;
}

class Sphere
{
    public Vector3 Center = new Vector3(0, 0, 0.5f);
    public float Radius = 0.4f;
    public Vector3 Color = Vector3.One;

    public Hit IntersectRayCollision(Ray ray)
    {
        Hit hit = new Hit { Distance = -1.0f };

        float b = 2.0f * Vector3.Dot(ray.Direction, ray.Start - this.Center);
        float c = Vector3.Dot(ray.Start - this.Center, ray.Start - this.Center)
                  - Radius * Radius;
        float nabla = b * b / 4.0f - c;

        if (nabla >= 0.0f)
        {
            float d1 = -b / 2.0f + MathF.Sqrt(nabla);
            float d2 = -b / 2.0f - MathF.Sqrt(nabla);

            hit.Distance = MathF.Min(d1, d2);
            hit.Point = ray.Start + ray.Direction * hit.Distance;
            hit.Normal = Vector3.Normalize(hit.Point - this.Center);
        }

        return hit;
    }
}

class Raytracer
{
    public Sphere Sphere = new Sphere();

    public Vector3 TransformScreenToWorld(int x, int y, int width, int height)
    {
        // 1. 가로는 4, 세로는 2 범위가 되도록 배율 계산
        float xScale = 4.0f / (width - 1);
        float yScale = 2.0f / (height - 1);

        // 2. 픽셀 좌표에 배율 적용: (0, 0) ~ (4, 2)
        float scaledX = x * xScale;
        float scaledY = y * yScale;

        // 3. 중앙을 원점으로 이동: (-2, -1) ~ (2, 1)
        float worldX = scaledX - 2.0f;
        float worldY = scaledY - 1.0f;
        float worldZ = 0.0f;

        return new Vector3(worldX, worldY, worldZ);
    }

    public Vector3 TraceRay(Ray ray)
    {
        Hit hit = Sphere.IntersectRayCollision(ray);
        if (hit.Distance < 0)
            return Vector3.Zero;

        // 원본과 같은 학습용 표현: 충돌 거리를 색에 곱합니다.
        return Sphere.Color * hit.Distance;
    }

    public Bitmap Render(int width, int height)
    {
        int[] pixels = new int[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Ray ray = new Ray
                {
                    Start = TransformScreenToWorld(x, y, width, height),
                    Direction = Vector3.UnitZ
                };
                Vector3 color = TraceRay(ray);
                int r = (int)(Math.Clamp(color.X, 0, 1) * 255);
                int g = (int)(Math.Clamp(color.Y, 0, 1) * 255);
                int b = (int)(Math.Clamp(color.Z, 0, 1) * 255);
                pixels[y * width + x] = (255 << 24) | (r << 16) | (g << 8) | b;
            }
        }

        // 계산한 픽셀 배열을 화면에 표시할 Bitmap으로 옮깁니다.
        Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        BitmapData data = image.LockBits(new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            for (int y = 0; y < height; y++)
                Marshal.Copy(pixels, y * width,
                    IntPtr.Add(data.Scan0, y * data.Stride), width);
        }
        finally
        {
            image.UnlockBits(data);
        }
        return image;
    }
}
