@echo off

set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"


%MSBuild% "%~dp0Deployment\Qube7.Composite.NuGet\Qube7.Composite.NuGet.csproj" -t:Restore
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Deployment\Qube7.Composite.NuGet\Qube7.Composite.NuGet.csproj" -t:Rebuild -p:Configuration=Release
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Deployment\Qube7.Composite.NuGet\Qube7.Composite.NuGet.csproj" -t:Pack -p:Configuration=Release -p:NuspecFile="%~dp0Deployment\Qube7.Composite.NuGet\QComposite.nuspec" -p:NuspecBasePath="%~dp0Deployment\Qube7.Composite.NuGet" -p:PackageOutputPath="%~dp0."
if "%ERRORLEVEL%" NEQ "0" goto :error

goto :end


:error
pause
exit /b 1

:end
exit /b 0
