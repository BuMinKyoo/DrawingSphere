# DrawingSphere

광선과 구의 교차 계산부터 Phong 조명, 원근투영, 삼각형 교차와 그림자까지 순서대로 공부하는 작은 C# Windows Forms 솔루션입니다. 각 단계는 독립된 프로젝트이며, 같은 창과 픽셀 그리기 구조를 사용합니다.

| 프로젝트 | 학습 내용 | 기준 |
| --- | --- | --- |
| [Step1_DrawingSphere](Step1_DrawingSphere/) | 정투영 광선과 구의 교차, 충돌 거리로 밝기 표현 | `470bbc8`의 소스 |
| [Step2_PhongShading](Step2_PhongShading/) | 정투영에 Ambient·Diffuse·Specular 조명 추가 | `eb51113`의 소스 |
| [Step3_PerspectiveView](Step3_PerspectiveView/) | 구 세 개의 최근접 충돌과 정투영·원근투영 비교 | 강의 Step6의 계산을 2단계 코드에 추가 |
| [Step4_Triangle](Step4_Triangle/) | 광선·평면 교점과 삼각형 내부 판정, 구와의 최근접 충돌 | 강의 Step7의 IntersectRayTriangle 흐름 |
| [Step5_Shadow](Step5_Shadow/) | 표면에서 그림자 광선을 만들고 가려진 곳에 Ambient만 적용 | 강의 Step8의 요청된 traceRay 식 |

## 실행

Windows와 .NET 10 SDK가 필요합니다. 외부 패키지는 사용하지 않습니다.

Visual Studio에서는 .NET 10을 지원하는 버전과 **.NET 데스크톱 개발** 워크로드를 사용합니다.

1. `DrawingSphere.sln`을 엽니다.
2. 솔루션 탐색기에서 실행할 프로젝트를 마우스 오른쪽 버튼으로 클릭합니다.
3. **시작 프로젝트로 설정**을 선택하고 `F5` 또는 `Ctrl+F5`로 실행합니다.

`run.cmd`를 더블 클릭해 1·2·3·4·5 중 실행할 단계를 선택할 수도 있습니다. 터미널에서는 다음 명령을 사용합니다.

```powershell
dotnet build DrawingSphere.sln -c Release
dotnet run --project Step1_DrawingSphere/Step1_DrawingSphere.csproj -c Release
dotnet run --project Step2_PhongShading/Step2_PhongShading.csproj -c Release
dotnet run --project Step3_PerspectiveView/Step3_PerspectiveView.csproj -c Release
dotnet run --project Step4_Triangle/Step4_Triangle.csproj -c Release
dotnet run --project Step5_Shadow/Step5_Shadow.csproj -c Release
```

## 단계별 조작

- **1단계:** 중심 X·Y·Z, 반지름과 색을 조절합니다. 흰색 구의 충돌 거리에 따라 밝기가 달라집니다.
- **2단계:** 기존 구 조절에 광원 X·Y·Z, 반짝임 지수 `Alpha`, 반짝임 세기 `Ks`를 더합니다. `확산색 (Diffuse)` 버튼으로 기본색을 바꿉니다.
- **3단계:** `원근 투영` 체크를 켜고 끄며 같은 장면을 비교합니다. 빨강·초록·파랑 구의 Z좌표와 광원 X·Y·Z를 조절합니다. 구의 반지름은 모두 `0.4`로 고정합니다.
- **4단계:** 빨간 구와 회색 삼각형을 함께 그립니다. 각각의 Z좌표, 광원 X·Y·Z를 조절하고 정투영·원근투영을 비교합니다. `삼각형 뒷면 제외`는 처음에는 꺼져 있습니다.
- **5단계:** 구 X·Y·Z와 광원 X·Y·Z를 조절합니다. `그림자` 체크를 켜고 끄며 바닥의 색을 비교합니다. 바닥은 삼각형 두 개로 만들고 위치를 고정합니다.
- 슬라이더의 값은 소수점 두 자리로 표시됩니다. 아래쪽 항목은 오른쪽 패널을 세로로 스크롤하면 보입니다.

1·2·3단계는 `Program.cs`, `Raytracer.cs`, `SphereWindow.cs` 세 소스 파일을 갖습니다. 4·5단계의 창 파일은 각각 `TriangleWindow.cs`, `ShadowWindow.cs`이며, 삼각형 계산을 `Triangle.cs`에서 읽을 수 있습니다. 공용 프로젝트나 프로젝트 간 참조는 없습니다.

## 처음 설정된 광선과 구 — 2단계 기준

| 항목 | 초기값 또는 계산 방법 |
| --- | --- |
| 렌더링 이미지 | 가로 640 × 세로 320 픽셀 |
| 광선 시작점 `Ray.Start` | 각 픽셀을 월드 좌표로 변환한 `(worldX, worldY, 0)` |
| 시작점의 X 범위 | `-2 ~ 2` |
| 시작점의 Y 범위 | `-1 ~ 1` — 화면 위쪽이 -1, 아래쪽이 +1 |
| 광선 방향 `Ray.Direction` | `(0, 0, 1)` — 길이가 1인 +Z 방향 |
| 구의 중심 `Sphere.Center` | `(0, 0, 0.5)` |
| 구의 반지름 `Sphere.Radius` | `0.4` |
| 구의 확산색 `Sphere.Color` | `(0, 0, 1)` — 파란색, 강의의 `diff`에 해당 |
| 주변광 성분 `Sphere.Ambient` | `(0, 0, 0)` |
| 반사광 색 `Sphere.Specular` | `(1, 1, 1)` — 흰색 |
| 반짝임 지수 `Sphere.Alpha` | `9` |
| 반짝임 세기 `Sphere.Ks` | `0.8` |
| 광원 위치 `Light.Position` | `(0, 0, -1)` |

아래 초기값과 광선 설명은 2단계 기준입니다. 1단계의 위치·반지름·광선은 같고 기본색만 흰색이며, 조명 필드는 없습니다.

구의 초기 설정은 다음과 같습니다.

```csharp
public Vector3 Center = new Vector3(0, 0, 0.5f);
public float Radius = 0.4f;
public Vector3 Color = new Vector3(0, 0, 1);
public Vector3 Ambient = Vector3.Zero;
public Vector3 Specular = Vector3.One;
public float Alpha = 9.0f;
public float Ks = 0.8f;
```

여기서 `(0, 0, 0.5)`는 **구의 중심**입니다. 월드 좌표계의 원점 `(0, 0, 0)`이 바뀌는 것은 아닙니다. 구를 구성하는 점을 배열에 미리 채우는 대신, 중심과 반지름으로 구 표면의 조건을 정의합니다.

광선은 픽셀마다 다음처럼 생성합니다.

```csharp
Ray ray = new Ray
{
    Start = TransformScreenToWorld(x, y, width, height),
    Direction = Vector3.UnitZ
};
```

C++ 예제의 `ray.start`와 `ray.dir`이 이 프로젝트의 `Start`와 `Direction`에 해당합니다. 방향은 모든 픽셀에서 같고, 시작 위치는 픽셀마다 다릅니다.

## 픽셀마다 레이저 하나를 쏜다고 생각하기 — 1·2단계

이미지의 각 픽셀에서 보이지 않는 레이저 하나를 +Z 방향으로 쏜다고 생각하면 됩니다. 구가 놓인 쪽으로 모두 평행하게 진행하며, 어떤 광선은 구를 만나고 어떤 광선은 빗나갑니다. 모든 광선이 구 중심을 향하도록 방향을 바꾸는 방식은 아닙니다.

1. 픽셀 좌표를 월드 좌표로 바꿉니다.
2. 그 위치를 시작점으로 삼아 +Z 방향 광선을 만듭니다.
3. 광선이 구 표면과 만나는지 계산합니다.
4. 교차 결과를 이용해 해당 픽셀의 색을 정합니다.

연속된 공간의 모든 점을 검사하는 것은 아닙니다. 현재 이미지의 640 × 320개 픽셀에 대응하는 시작점을 한 번씩 검사합니다. 평행한 광선을 사용하는 이 방식이 정투영에 해당합니다. 레이저는 계산을 상상하기 위한 비유입니다.

## 광선과 구의 공통점을 어떻게 구할까? — 1·2단계의 중앙 광선 예

광선 위의 점은 다음 식으로 나타냅니다.

```text
P(t) = Start + t × Direction    (t >= 0)
```

`t`는 시작점에서 방향 벡터를 얼마나 따라갔는지 나타내는 매개변수입니다. 현재 방향 벡터의 길이는 1이므로 `t`가 이동 거리와 같습니다. 현재 설정에서는 다음처럼 읽을 수 있습니다.

```text
Start     = (worldX, worldY, 0)
Direction = (0, 0, 1)
P(t)      = (worldX, worldY, t)
```

한 광선에서는 X와 Y가 고정되어 있고, Z만 증가합니다. 교차 계산에서 먼저 구하는 것은 공통 X좌표가 아니라, **구 표면에 도달하는 거리 t**입니다. 그 거리를 구한 뒤 `Start + t × Direction`으로 교차점의 3차원 좌표를 계산합니다.

구 표면은 중심 `C = (Cx, Cy, Cz)`에서의 거리가 반지름 `r`인 점들의 집합입니다.

```text
(x - Cx)² + (y - Cy)² + (z - Cz)² = r²

초기 구:
x² + y² + (z - 0.5)² = 0.4²
```

원의 방정식에서 Z축까지 확장한 구 표면의 방정식입니다. 이 식을 만족하는 점이 구의 표면에 놓입니다. 구 내부까지 포함한 영역은 등호 대신 `<=`로 표현할 수 있습니다.

광선의 식을 구의 식에 대입하면, 두 조건을 동시에 만족하는 `t`를 찾는 이차방정식이 됩니다. 방향 벡터의 길이가 1이므로 코드에서는 다음 식을 사용합니다.

```text
t² + b × t + c = 0

b = 2 × dot(Direction, Start - Center)
c = dot(Start - Center, Start - Center) - Radius²
nabla = b² / 4 - c

d1 = -b / 2 + sqrt(nabla)
d2 = -b / 2 - sqrt(nabla)
```

여기서 소문자 `c`는 이차방정식의 상수항이고, `Center`는 구의 중심입니다. `nabla`는 일반적인 판별식 `b² - 4c`를 4로 나눈 값입니다.

- `nabla < 0`: 직선과 구가 만나지 않습니다.
- `nabla = 0`: 구 표면에 한 점에서 접합니다.
- `nabla > 0`: 구 표면과 두 점에서 만납니다.

이 판정은 우선 무한한 직선과 구의 교차를 나타냅니다. 광선에서는 시작점 앞쪽의 교점이어야 하므로 거리의 부호도 확인해야 합니다.

### 중앙을 지나는 광선의 예

설명을 위해 정확히 중앙을 지나는 `Start = (0, 0, 0)`, `Direction = (0, 0, 1)`인 광선을 생각합니다.

```text
b = -1
c = 0.09
nabla = 0.16

d1 = 0.9
d2 = 0.1
```

구의 앞 표면과 뒤 표면을 만나는 두 거리입니다. 현재 코드는 작은 값인 `0.1`을 선택하므로 교차점은 `(0, 0, 0.1)`입니다.

```text
출발점          앞 표면         구 중심         뒤 표면
z=0 ---------- z=0.1 --------- z=0.5 --------- z=0.9
       +Z 방향으로 진행 →
```

실제 이미지는 짝수 해상도이므로 위 예시는 정확한 중앙점을 가정한 수학적 설명입니다.

구 교차 함수는 모든 단계에서 강의 예제처럼 두 근의 최솟값을 선택합니다. 선택한 거리가 음수이면 배경색을 반환하며, 시작점이 구 내부일 때 양수인 출구를 다시 선택하는 처리는 포함하지 않습니다. 1·2단계에서 슬라이더로 구가 시작점을 감싸게 만들면 해당 픽셀이 검게 표시될 수 있습니다.

## 1단계: 충돌 거리로 밝기 표현하기 — 강의 Step4

1단계에서는 다음 한 줄로 색을 정합니다.

```csharp
return Sphere.Color * hit.Distance;
```

1단계의 기본색이 흰색 `(1, 1, 1)`이고 거리가 `0.1`이면, 색은 `(0.1, 0.1, 0.1)`이 됩니다. 이 식은 거리가 클수록 밝아지는 깊이 확인용 표현입니다. 광원 위치나 표면 방향은 색 계산에 사용하지 않습니다.

## 2단계: Phong 조명으로 명암과 반짝임 표현하기 — 강의 Step5

2단계의 `TraceRay`에서는 교차점의 거리 대신 다음 세 성분을 더해 색을 정합니다. 3단계도 같은 Phong 계산을 사용합니다.

```text
색 = Ambient + Diffuse 색 × diffuse + Specular 색 × specular × Ks
```

교차 거리는 여전히 교차점 `Hit.Point`를 구하는 데 사용합니다. 그 교차점에서 광원까지의 방향과 구 표면의 법선 `Hit.Normal`을 이용해 밝기를 계산합니다.

### 1. Ambient: 기본으로 더하는 색

주변에서 들어오는 빛을 간단한 상수 색으로 표현합니다. 실제 주변 물체의 빛을 추적하는 계산은 아닙니다. 초기값은 검은색이므로 이 성분은 결과에 아무것도 더하지 않습니다.

### 2. Diffuse: 표면이 빛을 향하는 정도

`N`은 교차점에서 바깥쪽으로 향하는 단위 법선이고, `L`은 교차점에서 광원으로 향하는 단위 벡터입니다.

```text
N = hit.Normal
L = normalize(Light.Position - hit.Point)
diffuse = max(dot(N, L), 0)
```

법선과 광원 방향이 같으면 내적이 1이어서 밝고, 서로 수직이면 0이어서 어둡습니다. 음수는 0으로 제한합니다. 이 값을 구의 확산색 `Sphere.Color`에 곱합니다.

### 3. Specular: 반사 방향에 생기는 반짝임

`R`은 입사한 빛이 표면에서 반사되어 나가는 방향이고, `V`는 교차점에서 관찰자 쪽을 향하는 방향입니다. 현재 광선 방향의 반대쪽인 `-ray.Direction`을 사용합니다.

```text
R = 2 × dot(N, L) × N - L
V = -ray.Direction
specular = pow(max(dot(V, R), 0), Alpha)
```

반사 방향과 관찰 방향이 일치할수록 밝은 반짝임이 생깁니다. `Alpha`가 커질수록 반짝이는 영역이 좁아지고, `Ks`가 커질수록 반짝임이 강해집니다. 슬라이더 범위는 `Alpha = 1~100`, `Ks = 0~1`입니다. `Ks = 0`이면 이 성분은 사라집니다.

### 4. 세 성분을 더해 픽셀 색으로 만들기

```csharp
Vector3 ambientColor = Sphere.Ambient;
Vector3 diffuseColor = Sphere.Color * diffuse;
Vector3 specularColor = Sphere.Specular * specular * Sphere.Ks;

return ambientColor + diffuseColor + specularColor;
```

위의 정확한 중앙 광선 예에서는 교차점이 `(0, 0, 0.1)`이고, 법선과 광원 방향, 관찰 방향, 반사 방향이 모두 `(0, 0, -1)`입니다. 따라서 `diffuse = 1`, `specular = 1`이고 초기 색은 다음과 같습니다.

```text
(0, 0, 0) + (0, 0, 1) × 1 + (1, 1, 1) × 1 × 0.8
= (0.8, 0.8, 1.8)
```

마지막에 RGB를 각각 `0~1`로 제한하므로 `(0.8, 0.8, 1)`이 됩니다. 구를 만나지 않은 픽셀은 검은색입니다.

강의와 같은 기본 Phong 계산이며, 광원까지의 거리에 따른 빛의 감쇠나 다른 물체에 가려 생기는 그림자는 계산하지 않습니다. 구의 중심과 반지름, 좌표 변환, 교차점 계산은 기존 Step4 형태를 유지합니다.

## 3단계: 같은 크기의 구를 원근투영으로 보기 — 강의 Step6

구의 초기 위치와 색은 다음과 같습니다. 세 구의 반지름은 모두 `0.4`입니다.

| 구 | 중심 | Diffuse 색 |
| --- | --- | --- |
| 빨강 | `(0.5, 0, 0.5)` | `(1, 0.2, 0.2)` |
| 초록 | `(0, 0, 1.0)` | `(0.2, 1, 0.2)` |
| 파랑 | `(-0.5, 0, 1.5)` | `(0.2, 0.2, 1)` |

각 구는 `Ambient = (0.2, 0.2, 0.2)`, `Specular = (0.5, 0.5, 0.5)`, `Alpha = 10`, `Ks = 1`을 사용합니다. 광원은 `(0, 1, -1)`, 눈의 위치는 `(0, 0, -1.5)`입니다. 눈의 위치는 코드에 고정되어 있습니다.

### 광선 방향만 바꾸어 비교하기

픽셀을 월드 좌표로 바꾸는 4×2 범위와 `z = 0` 시작 평면은 앞 단계와 같습니다.

```csharp
Vector3 pixelPosWorld = TransformScreenToWorld(x, y, width, height);
Vector3 rayDirection = Vector3.UnitZ;

if (UsePerspective)
{
    Vector3 eyeToPixel = pixelPosWorld - EyePosition;
    rayDirection = Vector3.Normalize(eyeToPixel);
}

Ray ray = new Ray
{
    Start = pixelPosWorld,
    Direction = rayDirection
};
```

정투영에서는 모든 픽셀의 광선이 `(0, 0, 1)`로 평행합니다. 원근투영에서는 고정된 눈과 각 픽셀을 잇는 방향을 사용하므로 픽셀마다 광선의 기울기가 달라집니다. `normalize`는 그 방향을 유지하고 길이만 1로 맞춥니다.

시작점은 강의처럼 화면의 `pixelPosWorld`입니다. 광선을 뒤로 연장하면 같은 눈 위치에서 만나지만, 실제 충돌 검사는 화면부터 앞쪽으로 진행합니다. 따라서 눈과 화면 사이 물체는 검사하지 않고, `hit.Distance`도 눈이 아닌 화면의 각 픽셀에서부터 잰 거리입니다.

### 구의 크기는 어디서 달라질까?

구의 반지름을 줄이는 별도 코드는 없습니다. 각 픽셀마다 `b`, `c`, `nabla`로 구와의 교차를 검사하고, 광선 앞쪽에서 만난 픽셀에만 구의 색을 넣습니다.

원근 방향에서는 구가 멀어질수록 바깥쪽 픽셀의 광선이 구를 빗나가게 됩니다. 구와 만나는 픽셀의 범위가 좁아져 작게 보이는 것입니다. 체크를 해제한 정투영에서는 Z좌표를 바꾸어도 같은 반지름의 구는 같은 크기로 보입니다.

### 가장 가까운 구의 충돌 정보를 선택하기

`FindClosestCollision`은 구 세 개의 충돌 거리를 비교해 가장 가까운 양수 또는 0인 거리를 선택합니다. 배열은 일부러 파랑·초록·빨강 순서로 배치해, 첫 번째로 검사한 구를 바로 반환하는 방식과 구분합니다.

```csharp
if (hit.Distance >= 0.0f &&
    (closestHit.Distance < 0.0f || hit.Distance < closestHit.Distance))
{
    closestHit = hit;
    closestHit.Object = sphere;
}
```

충돌 정보 전체를 먼저 복사하고, 그다음 어떤 구에 맞았는지를 저장해야 합니다. 순서를 바꾸면 `hit.Object`의 빈 참조가 저장한 구 참조를 덮어씁니다. 아무 구에도 맞지 않으면 `Distance = -1`로 반환하고 검은색 배경을 사용합니다.

## 4단계: 광선으로 삼각형 그리기 — 강의 Step7

강의처럼 빨간 구 하나와 회색 삼각형 하나를 같은 장면에 둡니다. `FindClosestCollision`은 두 물체의 충돌 거리를 비교하고, 가장 가까운 물체의 재질로 Phong 조명을 계산합니다. 구와 삼각형의 공통 재질과 교차 함수는 작은 `SceneObject` 클래스로 묶었습니다.

| 항목 | 초기값 |
| --- | --- |
| 구 중심 / 반지름 | `(0.6, 0, 0.5)` / `0.4` |
| 삼각형 `V0` | `(-2, -2, 2)` |
| 삼각형 `V1` | `(-2, 2, 2)` |
| 삼각형 `V2` | `(2, 2, 2)` |
| 면 법선 `faceNormal` | `(0, 0, -1)` |
| 눈 / 광원 | `(0, 0, -1.5)` / `(0, 1, -1)` |
| 삼각형 재질 | `Color = (0.5, 0.5, 0.5)`, `Ambient = (0.2, 0.2, 0.2)`, `Specular = (0.5, 0.5, 0.5)`, `Alpha = 5`, `Ks = 1` |

### IntersectRayTriangle 읽기

강의의 순서와 벡터 계산을 유지했습니다.

1. `cross(v1 - v0, v2 - v0)`를 정규화해 면 법선을 구합니다.
2. 광선이 평면과 거의 평행하면 실패로 처리합니다.
3. 평면 교점까지의 거리 `t`를 구합니다. 음수이면 광선 뒤쪽이므로 실패입니다.
4. `point = orig + t * dir`로 평면과 만난 점을 구합니다.
5. 작은 삼각형 세 개의 법선을 구해 면 법선과 방향을 비교합니다. 하나라도 내적이 음수이면 삼각형 밖입니다.

```csharp
t = (Vector3.Dot(v0, faceNormal) - Vector3.Dot(orig, faceNormal))
    / Vector3.Dot(dir, faceNormal);
point = orig + t * dir;

Vector3 normal0 = Vector3.Cross(point - v2, v1 - v2);
Vector3 normal1 = Vector3.Cross(point - v0, v2 - v0);
Vector3 normal2 = Vector3.Cross(point - v1, v0 - v1);

if (Vector3.Dot(normal0, faceNormal) < 0.0f) return false;
if (Vector3.Dot(normal1, faceNormal) < 0.0f) return false;
if (Vector3.Dot(normal2, faceNormal) < 0.0f) return false;
```

작은 법선은 강의 주석처럼 `Normalize`를 생략했습니다. 내적의 부호는 같고, 교점이 변 위에 있어 외적이 0인 경우도 처리할 수 있습니다. 조명에 사용하는 면 법선의 정규화는 유지합니다. 강의에서 아직 계산하지 않는 텍스처용 `u`, `v` 인자는 생략했습니다.

뒷면 제외를 켜면 `dot(-dir, faceNormal) < 0`인 광선을 제외합니다. 초기 삼각형은 눈 쪽을 향하므로 켜고 꺼도 같은 그림입니다. `V1`과 `V2`의 순서를 바꿔 면 법선을 뒤집으면 차이를 확인할 수 있습니다. 체크를 끈 경우에는 강의처럼 법선 방향을 그대로 사용해 양쪽 면에 조명을 계산합니다.

픽셀 좌표 변환은 앞 단계처럼 화면 위가 Y=-1, 아래가 Y=+1입니다. 강의의 화면 Y 방향과 반대이므로 같은 정점이어도 화면에서 위아래가 반대로 보입니다.

## 5단계: 그림자 광선으로 가림 확인하기 — 강의 Step8

구 하나와 바닥을 구성하는 삼각형 두 개를 사용합니다. 기존의 삼각형 내부 판정과 Phong 계산을 이어서 사용하고, `TraceRay`에 그림자 판정을 추가했습니다.

| 항목 | 초기값 |
| --- | --- |
| 구 중심 / 반지름 | `(0, 0, 0.6)` / `0.4` |
| 구 재질 | `Ambient = (0.2, 0, 0)`, `Color = (1, 0.1, 0.1)`, `Specular = (1.5, 1.5, 1.5)`, `Alpha = 50` |
| 바닥 삼각형 1 | `(-2, -1, 0)`, `(-2, -1, 4)`, `(2, -1, 4)` |
| 바닥 삼각형 2 | `(-2, -1, 0)`, `(2, -1, 4)`, `(2, -1, 0)` |
| 바닥 재질 | `Ambient = (0.2, 0.2, 0.2)`, `Color = (0.8, 0.8, 0.8)`, `Specular = (1, 1, 1)`, `Alpha = 50` |
| 광원 | `(0, 1, 0.2)` |
| 눈 | `(0, 0, -1.5)` |
| 그림자 / 원근투영 | 모두 켜짐 |

두 바닥 삼각형은 Y=-1인 평면을 만들고 법선은 +Y를 향합니다. 강의처럼 바닥의 뒷면 제외를 사용합니다. 5단계의 화면 좌표 변환은 `worldY = 1 - scaledY`이므로 화면 위가 +Y, 아래가 -Y입니다. 앞 단계와 Y 방향이 다르며, 강의의 바닥 좌표를 그대로 아래쪽에 표시하기 위한 설정입니다.

### TraceRay의 계산 순서

1. 눈에서 화면을 통과하는 광선으로 가장 가까운 물체를 찾습니다. 실제 광선 시작점은 앞 단계처럼 화면 위의 픽셀입니다.
2. 교점에서 조금 이동한 위치를 시작점으로 그림자 광선을 만듭니다.
3. 그 광선이 물체에 닿으면 원래 교점 물체의 `Ambient`만 반환합니다.
4. 닿지 않으면 기존의 `Ambient + Diffuse + Specular`를 계산합니다.

요청한 C++ 식 `Ray shodowRay{ hit.point + 0.00001f, light.pos };`는 다음처럼 옮겼습니다. C#에서는 같은 값을 XYZ에 더하기 위해 `new Vector3(0.00001f)`를 사용합니다.

```csharp
Ray shadowRay = new Ray
{
    Start = hit.Point + new Vector3(0.00001f),
    Direction = Light.Position
};

Hit shadowHit = FindClosestCollision(shadowRay);
if (shadowHit.Distance >= 0.0f)
    return sceneObject.Ambient;
```

`UseShadows`가 켜졌을 때 이 부분을 실행합니다. 꺼졌을 때는 같은 장면에 기존 Phong 조명만 적용합니다. 바닥 그림자는 완전한 검정 대신 바닥의 주변광 색인 `(0.2, 0.2, 0.2)`가 됩니다.

### 요청한 식의 범위

이 단계는 보내주신 식을 그대로 옮긴 구현입니다. `Light.Position`을 광선 방향에 직접 넣으므로, 그림자 광선의 방향은 모든 교점에서 같습니다. 교점에서 광원으로 향하는 `Light.Position - hit.Point`를 사용하거나 방향을 정규화하는 계산은 포함하지 않습니다.

또한 구 교차 함수는 단위 방향을 가정하므로, 정규화하지 않은 그림자 광선에서는 교차 판정이 실제와 달라질 수 있습니다. 광원까지의 거리 제한도 없어서 광원 뒤의 충돌도 그림자로 판단할 수 있습니다. 광원을 움직일 때 정확한 점광원 그림자와 차이가 날 수 있는 이유입니다. `hit.Point + new Vector3(0.00001f)`는 모든 좌표에 작은 값을 더하는 방식이며, 법선 방향으로 이동하는 방식과도 다릅니다.

## 구 하나만 그릴 수 있는데 왜 Ray를 사용할까?

1·2단계처럼 고정된 방향으로 구 하나만 그리는 조건에서는 `Ray` 구조체를 따로 만들지 않아도 됩니다. 구의 방정식에서 각 X·Y 위치의 앞면 Z를 직접 계산하거나, 원에 명암을 넣는 방식으로도 구처럼 보이는 그림을 만들 수 있습니다.

기본 장면처럼 구가 시작 평면 앞에 있을 때는 다음처럼 앞면의 Z를 직접 구할 수도 있습니다.

```text
k = r² - (worldX - Cx)² - (worldY - Cy)²
frontZ = Cz - sqrt(k)    (k >= 0인 경우)
```

이 식은 현재 조건의 광선·구 교차 계산을 단순화해서 쓴 형태입니다. `Ray` 구조체 대신 시작점과 방향을 각각 함수 인자로 전달해도 같은 계산을 할 수 있습니다.

`Ray`는 “어디서 출발해 어느 방향으로 확인하는가”를 하나로 묶어 코드에 드러내는 역할을 합니다.

- 화면 좌표를 광선으로 만드는 부분과 구와의 교차를 계산하는 부분을 나누어 읽을 수 있습니다.
- 카메라나 물체가 바뀌어도 시작점·방향을 입력받아 교점을 구하는 공통 구조를 사용할 수 있습니다.
- 이후 다른 도형, 원근 카메라, 그림자나 반사를 공부할 때도 광선이라는 표현을 이어서 사용할 수 있습니다.

이 프로젝트에서는 `픽셀 → 3차원 광선 → 구 표면과의 교점 → 픽셀 색`의 연결을 배우기 위해 `Ray`를 사용합니다.

## 코드 읽는 순서

각 단계의 폴더 안에서 다음 순서로 읽습니다.

1. `Program.cs`: 프로그램 시작과 창 실행
2. `Raytracer.cs`: 광선과 물체의 교차 계산, 해당 단계의 색 계산과 픽셀 생성
3. `SphereWindow.cs`, `TriangleWindow.cs` 또는 `ShadowWindow.cs`: 슬라이더·체크박스 입력과 화면 표시

3단계에서는 `Render → TraceRay → FindClosestCollision → IntersectRayCollision` 순서로 호출을 따라가면 원근투영과 교차 판정의 연결을 볼 수 있습니다.

4단계에서는 `Render → TraceRay → FindClosestCollision → Triangle.IntersectRayCollision → IntersectRayTriangle` 순서로 삼각형 계산을 따라갈 수 있습니다.

5단계에서는 `TraceRay` 안에서 `FindClosestCollision`이 화면 광선과 그림자 광선에 대해 각각 호출되는 흐름을 확인할 수 있습니다.

구 그리기 강의의 학습 주제를 바탕으로 Codex의 도움을 받아 작성했습니다.

## 참고 자료

- [Line–sphere intersection — Wikipedia](https://en.wikipedia.org/wiki/Line-sphere_intersection): 직선과 구의 교차 방정식 참고
- [Phong Illumination Model — Scratchapixel](https://www.scratchapixel.com/lessons/3d-basic-rendering/phong-shader-BRDF/phong-illumination-models-brdf.html): Diffuse와 Specular 계산 참고
- [Ray–Triangle Intersection — Scratchapixel](https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/ray-triangle-intersection-geometric-solution.html): 평면 교점과 삼각형 내부 판정 참고
