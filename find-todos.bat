@echo off
echo ========================================
echo TODO Finder - Scanning for TODOs...
echo ========================================
echo.

cd /d "%~dp0"
dotnet run --project TodoAttribute.Sol\TodoFinder\TodoFinder.csproj

echo.
echo ========================================
echo Press any key to exit...
pause >nul
