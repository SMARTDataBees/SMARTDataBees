echo Builds all Solutions

echo The Core
cd ..\SDBees.Core\Batches\
call NAnt_Build_Core.bat
cd ..\..\Batches

echo AEC Bees
cd ..\SDBees.AECBees\Batches\
call NAnt_Build_AEC.bat
cd ..\..\Batches

echo Connectivity
cd ..\SDBees.Connectivity\Batches\
call NAnt_Build_Connectivity.bat
cd ..\..\Batches

echo CRM TBD

echo General
cd ..\SDBees.General\Batches\
call NAnt_Build_General.bat
cd ..\..\Batches

echo GISBees
cd ..\SDBees.GISBees\Batches\
call NAnt_Build_GIS.bat
cd ..\..\Batches