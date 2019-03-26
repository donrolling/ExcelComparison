param([string]$path, [string]$WEBSITEURL)
$path -replace ' ', '` '

$newFile = "$path.bak"

$find = "{{WEBSITEURL}}"
$replace = "$WEBSITEURL"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path