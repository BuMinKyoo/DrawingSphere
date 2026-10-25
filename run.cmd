@echo off
echo 1. DrawingSphere
echo 2. PhongShading
echo 3. PerspectiveView
echo 4. Triangle
choice /c 1234 /n /m "Select a step [1/2/3/4]: "
if errorlevel 4 (
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
