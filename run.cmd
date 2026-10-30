@echo off
echo 1. DrawingSphere
echo 2. PhongShading
echo 3. PerspectiveView
echo 4. Triangle
echo 5. Shadow
echo 6. Supersampling
choice /c 123456 /n /m "Select a step [1/2/3/4/5/6]: "
if errorlevel 6 (
    set "taskProject=Step6_Supersampling\Step6_Supersampling.csproj"
) else if errorlevel 5 (
    set "taskProject=Step5_Shadow\Step5_Shadow.csproj"
) else if errorlevel 4 (
    set "taskProject=Step4_Triangle\Step4_Triangle.csproj"
) else if errorlevel 3 (
    set "taskProject=Step3_PerspectiveView\Step3_PerspectiveView.csproj"
) else if errorlevel 2 (
    set "taskProject=Step2_PhongShading\Step2_PhongShading.csproj"
) else (
    set "taskProject=Step1_DrawingSphere\Step1_DrawingSphere.csproj"
)
dotnet run --project "%~dp0%taskProject%" -c Release
if errorlevel 1 (
    pause
    exit /b 1
)
