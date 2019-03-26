$configPath = 'CodeGenerator\config.json'
$configExists = Test-Path -Path $configPath

if(!$configExists){
	$parentDir = (get-item '.\').parent.FullName
	$configPath = "$parentDir\config.json"
	$configExists = Test-Path -Path $configPath
	if(!$configExists){
		Write-Host "Cannot locate configuration file"
	}
} else {
	Write-Host $configPath
}

$json = (Get-Content $configPath) -join "`n" | ConvertFrom-Json
$outputDirectory = $json | Select -expand OutputDirectory
$destinationPath = $json | Select -expand DestinationDirectory

$path = "$outputDirectory\Business\"
$exists = Test-Path -Path $path
if(!$exists){
	New-Item -ItemType Directory -Force -Path $path
}
Write-Host "Path: $path Destination: $destinationPath"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Data\"
$exists = Test-Path -Path $path
if(!$exists){
	New-Item -ItemType Directory -Force -Path $path
}
Write-Host "Path: $path Destination: $destinationPath"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Models\"
$exists = Test-Path -Path $path
if(!$exists){
	New-Item -ItemType Directory -Force -Path $path
}
Write-Host "Path: $path Destination: $destinationPath"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Tests\"
$exists = Test-Path -Path $path
if(!$exists){
	New-Item -ItemType Directory -Force -Path $path
}
Write-Host "Path: $path Destination: $destinationPath"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force
