@echo off

if .%2.==.. goto usage

set LogFileName=%1.Build.log

del %LogFileName%

msbuild %1.sln /t:Rebuild /p:Configuration=%2 /noconlog /l:FileLogger,Microsoft.Build.Engine;logfile=%LogFileName%;append=false;verbosity=normal;encoding=utf-8

start notepad %LogFileName%

goto ende

:usage

@echo Usage:
@echo BuildSld SolutionNameOhneExtension Config
@echo Beispiel: BuildSln SDBees.Core\SDBees.Core Debug

:ende
