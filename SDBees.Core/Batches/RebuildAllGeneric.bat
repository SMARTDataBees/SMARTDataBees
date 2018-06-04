@echo off

if .%1.==.. goto usage


call "c:\Programme\Microsoft Visual Studio 8\Common7\Tools\vsvars32.bat"

set Config=%1%

rem Go one directory up, so that we are in SmartDataBees
cd ..

@echo Building SDBees.Core...
cd SDBees.Core\Batches
del ..\run\*.dll
cd ..\..
call Batches\BuildSln SDBees.Core\SDBees.Core %Config%

@echo Building SDBees.General...
cd SDBees.General\Batches
del ..\run\*.dll
call "copy core.cmd"
cd ..\..

call Batches\BuildSln SDBees.General\SDBees.General %Config%

@echo Building SDBees.AECBees...
cd SDBees.AECBees\Batches
del ..\run\*.dll
call "copy core.cmd"
cd ..\..

call Batches\BuildSln SDBees.AECBees\SDBees.AEC\AECBees %Config%
call Batches\BuildSln SDBees.AECBees\SDBees.HVAC\HVAC %Config%
call Batches\BuildSln SDBees.AECBees\SDBees.Organisation\Orga %Config%
call Batches\BuildSln SDBees.AECBees\SDBees.Process\Process %Config%


@echo Building SDBees.GISBees...
cd SDBees.GISBees\Batches
del ..\run\*.dll
call "copy core.cmd"
cd ..\..

call Batches\BuildSln SDBees.GISBees\SDBees.GISBees %Config%


@echo Building SDBees.Connectivity...
cd SDBees.Connectivity\Batches
del ..\run\*.dll
call "copy core.cmd"
cd ..\..

call Batches\BuildSln SDBees.Connectivity\SDBeesConnectivity %Config%


@echo Building SDBees.CRM...
cd SDBees.CRM\Batches
del ..\run\*.dll
call "copy core.cmd"
cd ..\..

call Batches\BuildSln SDBees.CRM\CRMBees %Config%

pause:


goto ende

:usage

@echo Usage:
@echo RebuildAllGeneric Config
@echo Beispiel: RebuildAllGeneric Debug

:ende
