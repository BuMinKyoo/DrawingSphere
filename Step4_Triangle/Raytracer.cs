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
    public SceneObject? Object;
}

class Light
{
    public Vector3 Position = new Vector3(0, 1, -1);
}

abstract class SceneObject
{
    public Vector3 Color = new Vector3(0.5f);
    public Vector3 Ambient = new Vector3(0.2f);
    public Vector3 Specular = new Vector3(0.5f);
    public float Alpha = 10.0f;
    public float Ks = 1.0f;

    public abstract Hit IntersectRayCollision(Ray ray);
}

class Sphere : SceneObject
{
    public Vector3 Center = new Vector3(0, 0, 0.5f);
    public float Radius = 0.4f;

    public override Hit IntersectRayCollision(Ray ray)
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
    public Sphere Sphere = new Sphere
    {
        Center = new Vector3(0.6f, 0, 0.5f),
        Color = new Vector3(1, 0.1f, 0.1f),
        Ambient = new Vector3(0.1f),
        Specular = Vector3.One,
        Alpha = 50.0f
    };
    public Triangle Triangle = new Triangle();
    public SceneObject[] Objects;
    public Light Light = new Light();
    public Vector3 EyePosition = new Vector3(0, 0, -1.5f);
    public bool UsePerspective = true;

    public Raytracer()
    {
        Objects = new SceneObject[] { Sphere, Triangle };
    }

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

    public Hit FindClosestCollision(Ray ray)
    {
        Hit closestHit = new Hit { Distance = -1.0f };

        foreach (SceneObject sceneObject in Objects)
        {
            Hit hit = sceneObject.IntersectRayCollision(ray);
            if (hit.Distance >= 0.0f &&
                (closestHit.Distance < 0.0f || hit.Distance < closestHit.Distance))
            {
                // 충돌 정보를 먼저 복사한 뒤, 그 충돌의 물체를 저장합니다.
                closestHit = hit;
                closestHit.Object = sceneObject;
            }
        }

        return closestHit;
    }

    public Vector3 TraceRay(Ray ray)
    {
        Hit hit = FindClosestCollision(ray);
        if (hit.Distance < 0 || hit.Object == null)
            return Vector3.Zero;

        SceneObject sceneObject = hit.Object;

        // 1. 교차점에서 광원으로 향하는 단위 벡터
        Vector3 toLight = Light.Position - hit.Point;
        if (toLight.LengthSquared() == 0.0f)
            return sceneObject.Ambient;
        Vector3 dirToLight = Vector3.Normalize(toLight);

        // 2. Diffuse: 표면이 빛을 향하는 정도
        float normalDotLight = Vector3.Dot(hit.Normal, dirToLight);
        float diffuse = MathF.Max(normalDotLight, 0.0f);

        // 3. Specular: 반사 방향과 관찰 방향이 일치하는 정도
        Vector3 reflectDir = 2.0f * normalDotLight * hit.Normal - dirToLight;
        Vector3 viewDir = -ray.Direction;
        float viewDotReflect = Vector3.Dot(viewDir, reflectDir);
        float specularBase = MathF.Max(viewDotReflect, 0.0f);
        float specular = MathF.Pow(specularBase, sceneObject.Alpha);

        // 4. Ambient + Diffuse + Specular
        Vector3 ambientColor = sceneObject.Ambient;
        Vector3 diffuseColor = sceneObject.Color * diffuse;
        Vector3 specularColor = sceneObject.Specular * specular * sceneObject.Ks;

        return ambientColor + diffuseColor + specularColor;
    }

    public Bitmap Render(int width, int height)
    {
        int[] pixels = new int[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pixelPosWorld = TransformScreenToWorld(x, y, width, height);
                Vector3 rayDirection = Vector3.UnitZ;

                if (UsePerspective)
                {
                    // 눈은 고정되어 있지만 픽셀 위치가 달라지므로 방향도 달라집니다.
                    Vector3 eyeToPixel = pixelPosWorld - EyePosition;
                    rayDirection = Vector3.Normalize(eyeToPixel);
                }

                Ray ray = new Ray
                {
                    Start = pixelPosWorld,
                    Direction = rayDirection
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
