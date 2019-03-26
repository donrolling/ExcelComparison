Start-Process -FilePath "CodeGenerator.CLI.exe" -Wait
$json = (Get-Content 'config.json') -join "`n" | ConvertFrom-Json
$postProcessScripts = $json | Select -expand PostProcessScripts
Write-Host $outputDirectory
foreach ($script in $postProcessScripts){ 
	$command = ".\Scripts\$script"
	Invoke-Expression "$command"
}