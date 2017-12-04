echo "meme"
wget "https://ci.appveyor.com/api/projects/NaamloosDT/modcore/artifacts/ModCore/bin/Release/ModCore%20Release%20Build.zip" -O update.zip
unzip -o update.zip
nohup dotnet ModCore.dll