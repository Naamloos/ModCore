#!/bin/bash

echo "Waiting for process to terminate"
wait "$1"

echo "Beginning update package download"
curl -LO "https://ci.appveyor.com/api/projects/NaamloosDT/modcore/artifacts/ModCore.zip"

echo "Beginning archive extraction"
unzip -o ModCore.zip

echo "Restarting process"
dotnet ModCore.dll $2 $3
