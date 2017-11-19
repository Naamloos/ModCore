$client = new-object System.Net.WebClient
$client.DownloadFile("https://ci.appveyor.com/api/projects/NaamloosDT/modcore/artifacts/ModCore/bin/Release/ModCore%20Release%20Build.zip","update.zip")