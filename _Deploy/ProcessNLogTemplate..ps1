param([string]$path, [string]$FOLDERNAME)
$path -replace ' ', '` '

$newFile = "$path.bak"

$find = "{{FOLDERNAME}}"
$replace = "$FOLDERNAME"
filter regexp { $_ -replace $find, $replace }
get-content $path | regexp | Set-Content $newFile

Remove-Item $path
Rename-Item $newFile $path