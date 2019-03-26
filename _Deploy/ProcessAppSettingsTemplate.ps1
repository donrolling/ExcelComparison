param([string]$path, [string]$BuildNumber, [string]$SQLSERVER, [string]$DatabaseName, [string]$WebsiteUrl)
$newFile = "$path.bak"

$path -replace ' ', '` '

if ($BuildNumber -like '*Release*') {
	$r1 = $BuildNumber.Split('_')[2];
	$BuildNumber = $r1.replace("Release", "").trim('.')
}
$find = "{{BuildNumber}}"
$replace = "$BuildNumber"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path

$find = "{{SQLSERVER}}"
$replace = "$SQLSERVER"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path

$find = "{{DatabaseName}}"
$replace = "$DatabaseName"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path

$find = "{{WebsiteUrl}}"
$replace = "$WebsiteUrl"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path