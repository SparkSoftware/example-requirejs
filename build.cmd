@ECHO OFF

set Target=%1
set MSBuildExe="%ProgramFiles(x86)%\MSBuild\12.0\Bin\msbuild.exe"

IF (%1)==() SET Target=Build
IF NOT EXIST %MSBuildExe% GOTO MSBuildNotFound

:: Build product
:: --------------------------------------------------
:Build

cd src
"%ProgramFiles(x86)%\MSBuild\12.0\Bin\msbuild.exe" build.proj /verbosity:minimal /target:%Target%
if errorlevel 1 pause
cd..

GOTO Exit

:MSBuildNotFound
ECHO MSBuild Not Found: %MSBuildExe%
PAUSE
GOTO Exit

:Exit
