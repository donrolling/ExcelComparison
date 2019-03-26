Start-Process -FilePath "CodeGenerator\CodeGenerator.CLI.exe" -Wait
$json = (Get-Content 'CodeGenerator\config.json') -join "`n" | ConvertFrom-Json
$postProcessScripts = $json | Select -expand PostProcessScripts
Write-Host $outputDirectory
foreach ($script in $postProcessScripts){ 
	$command = ".\CodeGenerator\Scripts\$script"
	Invoke-Expression "$command"
}