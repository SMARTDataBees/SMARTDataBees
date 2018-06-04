REM Die Dateien für die Debugumgebung
xcopy /Y ..\..\..\SmartDataBees.Core\run\*.dll ..\run
xcopy /Y ..\..\..\SmartDataBees.Core\run\*.exe ..\run
xcopy /Y ..\..\..\SmartDataBees.Core\run\*.config ..\run
xcopy /Y ..\..\..\SmartDataBees.Core\run\icon\*.ico ..\run\icon

REM Die Referenzen kopieren
xcopy /Y ..\..\..\SmartDataBees.Core\run\*.dll ..\SDBees.Core.Build