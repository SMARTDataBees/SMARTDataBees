
REM Die AECBees
xcopy /Y ..\..\SDBees.AECBees\run\*AEC*.dll ..\run
xcopy /Y ..\..\SDBees.AECBees\run\*HVAC*.dll ..\run
xcopy /Y ..\..\SDBees.AECBees\run\*Orga*.dll ..\run
xcopy /Y ..\..\SDBees.AECBees\run\*Process*.dll ..\run
xcopy /Y ..\..\SDBees.AECBees\run\Icons\*.ico ..\run\Icons

REM Die GISBees
xcopy /Y ..\..\SDBees.GISBees\run\*GIS*.dll ..\run
xcopy /Y ..\..\SDBees.GISBees\run\Icons\*.ico ..\run\Icons

REM Die Generellen Sachen
xcopy /Y ..\..\SDBees.General\run\*General*.dll ..\run
xcopy /Y ..\..\SDBees.General\run\Icons\*.ico ..\Icons

REM Die Connectivity
xcopy /Y ..\..\SDBees.Connectivity\run\*Connectivity*.dll ..\run
xcopy /Y ..\..\SDBees.Connectivity\run\Icons\*.ico ..\run\Icons



Pause