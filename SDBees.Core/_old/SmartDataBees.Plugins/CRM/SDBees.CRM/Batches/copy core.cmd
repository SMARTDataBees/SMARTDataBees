REM Die Dateien für die Debugumgebung
xcopy /Y ..\..\SDBees.Core\run\*.dll ..\run
xcopy /Y ..\..\SDBees.Core\run\*.exe ..\run
xcopy /Y ..\..\SDBees.Core\run\*.config ..\run

REM Die Referenzen
xcopy /Y ..\..\SDBees.Core\run\*.dll ..\SDBees.Core.Build
