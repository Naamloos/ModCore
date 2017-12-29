Wait 5
Powershell.exe -executionpolicy remotesigned -File download.ps1
"C:\Program Files\7-Zip\7z.exe" e update.zip -ao
start dotnet ModCore.dll %1 %2
