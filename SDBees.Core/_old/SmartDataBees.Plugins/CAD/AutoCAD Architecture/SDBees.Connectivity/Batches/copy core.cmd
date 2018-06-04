REM Die Dateien für die Debugumgebung
xcopy /Y ..\..\SDBees.Core\run\*.dll ..\run
xcopy /Y ..\..\SDBees.Core\run\*.exe ..\run
xcopy /Y ..\..\SDBees.Core\run\*.config ..\run
xcopy /Y ..\..\SDBees.AECBees\run\*AEC*.dll ..\run
xcopy /Y ..\..\SDBees.AECBees\run\*HVAC*.dll ..\run

REM Die Referenzen
xcopy /Y ..\..\SDBees.Core\run\*.dll ..\SDBees.Core.Build
REM xcopy /Y ..\..\SDBees.AECBees\run\*AEC*.dll ..\SDBees.Core.Build
