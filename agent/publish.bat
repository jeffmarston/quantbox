

dotnet publish --self-contained --framework netcoreapp2.2 -r win-x64 

copy install-service.bat .\bin\Debug\netcoreapp2.2\win-x64\publish

"C:\Program Files\7-Zip\7z.exe" a EzeAgent.zip -w .\bin\Debug\netcoreapp2.2\win-x64\publish -mem=AES256
