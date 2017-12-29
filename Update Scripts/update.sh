#!/bin/bash
if [ -z "$3" ]
then
    echo "Relaunching"
    nohup bash "$0" "$1" "$2" "yee"
    exit
fi

echo "Waiting for process to terminate"
wait "$1"

echo "Beginning update package download"
curl -LO "https://ci.appveyor.com/api/projects/NaamloosDT/modcore/artifacts/ModCore/bin/Release/ModCore%20Release%20Build.zip" -o update.zip

echo "Beginning archive extraction"
unzip -o ModCore%20Release%20Build.zip

echo "Restarting process"
nohup dotnet ModCore.dll $2
