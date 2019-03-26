param([string]$sqlPackagePath, [string]$sqlServerName, [string]$databaseName, [string]$dacpacPath, [string]$dacFile, [string[]]$additionalDacpacs)
$upgradeExisting = $true
add-type -path $sqlPackagePath

$cn = "Data Source=$sqlServerName;Initial Catalog=$databaseName;Integrated Security=true;"
$dacService = new-object Microsoft.SqlServer.Dac.DacServices $cn
$currentDatabaseDacFile = $dacpacPath + $dacFile
$dacpac = [Microsoft.SqlServer.Dac.DacPackage]::Load($currentDatabaseDacFile)

$excludeObjectTypes = 'Users','Logins','RoleMembership','Permissions','Credentials','DatabaseScopedCredentials', 'ServerRoles', 'ServerRoleMembership', 'DatabaseRoles', 'Credentials', 'ApplicationRoles'
$deployOptions = new-object Microsoft.SqlServer.Dac.DacDeployOptions
$deployOptions.IncludeCompositeObjects = $false
$deployOptions.IgnoreFileSize = $false
$deployOptions.IgnoreFilegroupPlacement = $false
$deployOptions.IgnoreFileAndLogFilePath = $false
$deployOptions.IgnorePermissions = $true
$deployOptions.IgnoreUserSettingsObjects = $true
$deployOptions.IgnoreLoginSids = $true
$deployOptions.AllowIncompatiblePlatform = $false
$deployOptions.DropObjectsNotInSource = $true
$deployOptions.DoNotDropObjectTypes = $excludeObjectTypes
$deployOptions.ExcludeObjectTypes = $excludeObjectTypes

Foreach($additionalDacpac in $additionalDacpacs){
    $nameParts = $additionalDacpac.Split('.')
    $thisDacPath = $dacpacPath + $additionalDacpac
    $deployOptions.SqlCommandVariableValues.Add($nameParts[0], $thisDacPath)
}

Register-ObjectEvent -InputObject $dacService -EventName "Message" -Action { Write-Host $EventArgs.Message.Message } | out-null

try {
    $dacService.deploy($dacpac, $databaseName, $upgradeExisting, $deployOptions)
} catch {
    Write-Host "LoadException";
    $Error | format-list -force;
    throw $Error[0].Exception.ParentContainsErrorRecordException;
}