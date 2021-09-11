@echo off

set MSBuild="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"


%MSBuild% "%~dp0Qube7.Composite.Vsix\Qube7.Composite.Vsix.csproj" -t:Restore
if "%ERRORLEVEL%" NEQ "0" goto :error
%MSBuild% "%~dp0Qube7.Composite.Vsix\Qube7.Composite.Vsix.csproj" -t:Rebuild -p:Configuration=Release -p:OutputPath="%~dp0."
if "%ERRORLEVEL%" NEQ "0" goto :error

goto :end


:error
pause
exit /b 1

:end
exit /b 0
