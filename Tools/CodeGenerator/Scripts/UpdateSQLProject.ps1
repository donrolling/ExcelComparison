$fullPathToFiles = (get-item '.\').FullName
$configFile = 'config.json'
$configPath = "$fullPathToFiles\CodeGenerator\$configFile"

$configExists = Test-Path -Path $configPath
if(!$configExists){
	$fullPathToFiles = (get-item '.\').parent.FullName
	$configPath = "$fullPathToFiles\$configFile"
	$configExists = Test-Path -Path $configPath
	Write-Host $configPath
	if(!$configExists){
		Write-Host "Cannot locate configuration file"
	}
}

$commandScriptFile = "runSQL.ps1"
$commandScriptPath = "$fullPathToFiles\CodeGenerator\Scripts\$commandScriptFile"
$serverParam = "Server="
$catalogParam = "Initial Catalog="
$configExists = Test-Path -Path $configPath
if(!$configExists){
	Write-Error "Cannot locate configuration file"
	return
}
$json = (Get-Content $configPath) -join "`n" | ConvertFrom-Json
$outputDirectory = $json | Select -expand OutputDirectory
$connectionString = $json | Select -expand ConnectionString
Write-Host $outputDirectory
Write-Host $connectionString

$hasMatch = $connectionString -Match $serverParam
if(!$hasMatch){
	Write-Error "Connection string is in the wrong format. Does not contain Server property. Example: 'Server=ax-dev10;Initial Catalog=MessageRouter;Integrated Security=SSPI;'"
	return
}

$connectionStringLen = ($connectionString).Length
$serverParamLen = ($serverParam).Length
$serverPosStart = $connectionString.IndexOf($serverParam) + $serverParamLen
$serverPosEnd = $connectionString.IndexOf(';')
$server = $connectionString.Substring($serverPosStart, $serverPosEnd - $serverPosStart)
Write-Host $server

$hasMatch = $connectionString -Match $catalogParam
if(!$hasMatch){
	Write-Error "Connection string is in the wrong format. Does not contain Initial Catalog property. Example: 'Server=ax-dev10;Initial Catalog=MessageRouter;Integrated Security=SSPI;'"
	return
}
$catalogParamLen = ($catalogParam).Length
$catalogPosStart = $connectionString.IndexOf($catalogParam) + $catalogParamLen
$catalogPosEnd = $connectionString.Substring($catalogPosStart, $connectionStringLen - $catalogPosStart).IndexOf(';') + $catalogPosStart
$database = $connectionString.Substring($catalogPosStart, $catalogPosEnd - $catalogPosStart)

$path = "$outputDirectory\Database\Drop"
Write-Host $path
Invoke-Expression "$commandScriptPath -server $server -database $database -path $path"
Write-Host 'OK'

$path = "$outputDirectory\Database\Functions"
Write-Host $path
Invoke-Expression "$commandScriptPath -server $server -database $database -path $path"
Write-Host 'OK'

$path = $outputDirectory + '\Database\Stored` Procedures'
Write-Host $path
Invoke-Expression "$commandScriptPath -server $server -database $database -path $path"
Write-Host 'OK'