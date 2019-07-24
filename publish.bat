mkdir blackboxsim\server
mkdir blackboxsim\client\dist

cd client
REM npm run build 
xcopy dist ..\blackboxsim\client\dist /Y /S

cd ..\server
dotnet build
dotnet publish -r win-x64 -c Release --self-contained
xcopy bin\Release\netcoreapp2.2\win-x64\publish ..\blackboxsim\server /Y /S
cd ..

copy install.bat .\blackboxsim\  /Y 
copy readme.md .\blackboxsim\  /Y

REM "C:\Program Files\7-Zip\7z.exe" a blackboxsim_setup.exe -mmt -mx5 -sfx blackboxsim\

REM xcopy blackboxsim_setup.exe c:\share\ /Y /S