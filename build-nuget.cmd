@echo off

set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"


%MSBuild% "%~dp0Qube7.Common\Qube7.Common.csproj" -t:Restore
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Qube7.Common\Qube7.Common.csproj" -t:Rebuild -p:Configuration=Release
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Qube7.Common\Qube7.Common.csproj" -t:Pack -p:Configuration=Release -p:PackageOutputPath="%~dp0."
if "%ERRORLEVEL%" NEQ "0" goto :error

%MSBuild% "%~dp0Qube7.Composite\Qube7.Composite.csproj" -t:Restore
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Qube7.Composite\Qube7.Composite.csproj" -t:Rebuild -p:Configuration=Release
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Qube7.Composite\Qube7.Composite.csproj" -t:Pack -p:Configuration=Release -p:PackageOutputPath="%~dp0."
if "%ERRORLEVEL%" NEQ "0" goto :error

goto :end


:error
pause
exit /b 1

:end
exit /b 0
