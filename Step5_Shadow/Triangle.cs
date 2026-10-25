using System;
using System.Numerics;

// 평면과의 교점을 구한 뒤, 그 점이 삼각형 안에 있는지 확인합니다.
class Triangle : SceneObject
{
    public Vector3 V0 = new Vector3(-2, -2, 2);
    public Vector3 V1 = new Vector3(-2, 2, 2);
    public Vector3 V2 = new Vector3(2, 2, 2);
    public bool CullBackFaces = false;

    public Triangle()
    {
        Alpha = 5.0f;
    }

    public override Hit IntersectRayCollision(Ray ray)
    {
        Hit hit = new Hit { Distance = -1.0f };

        if (IntersectRayTriangle(ray.Start, ray.Direction, V0, V1, V2,
            out Vector3 point, out Vector3 faceNormal, out float t))
        {
            hit.Distance = t;
            hit.Point = point;
            hit.Normal = faceNormal;
        }

        return hit;
    }

    public bool IntersectRayTriangle(Vector3 orig, Vector3 dir,
        Vector3 v0, Vector3 v1, Vector3 v2,
        out Vector3 point, out Vector3 faceNormal, out float t)
    {
        point = Vector3.Zero;
        t = 0.0f;

        // 1. 삼각형이 놓여 있는 평면의 수직 벡터 계산
        faceNormal = Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));

        // 강의의 주석 처리된 검사에 해당합니다. 처음에는 사용하지 않습니다.
        if (CullBackFaces && Vector3.Dot(-dir, faceNormal) < 0.0f)
            return false;

        // 평면과 광선이 거의 평행하면 충돌하지 않는 것으로 판단합니다.
        if (MathF.Abs(Vector3.Dot(dir, faceNormal)) < 1e-2f)
            return false;

        // 2. 광선과 평면의 충돌 위치 계산
        t = (Vector3.Dot(v0, faceNormal) - Vector3.Dot(orig, faceNormal))
            / Vector3.Dot(dir, faceNormal);

        if (t < 0.0f)
            return false;

        point = orig + t * dir;

        // 3. 충돌점이 삼각형 안에 있는지 확인
        // 강의 주석처럼 방향만 비교하므로 작은 법선의 Normalize는 생략합니다.
        Vector3 normal0 = Vector3.Cross(point - v2, v1 - v2);
        Vector3 normal1 = Vector3.Cross(point - v0, v2 - v0);
        Vector3 normal2 = Vector3.Cross(point - v1, v0 - v1);

        if (Vector3.Dot(normal0, faceNormal) < 0.0f) return false;
        if (Vector3.Dot(normal1, faceNormal) < 0.0f) return false;
        if (Vector3.Dot(normal2, faceNormal) < 0.0f) return false;

        return true;
    }
}
