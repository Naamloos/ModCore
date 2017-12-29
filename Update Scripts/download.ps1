$client = new-object System.Net.WebClient
$client.DownloadFile("https://ci.appveyor.com/api/projects/NaamloosDT/modcore/artifacts/ModCore.zip","update.zip")