@echo off
dotnet run --project "%~dp0SimpleDrawingSphere.csproj" -c Release
if errorlevel 1 (
    pause
    exit /b 1
)
