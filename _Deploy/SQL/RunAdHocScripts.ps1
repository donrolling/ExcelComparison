param([string]$sqlServerName, [string]$databaseName, [string]$pathToScripts, [string[]]$scripts)
#Example to test if SQL snap-ins are present
    #get-pssnapin -Registered
#Example call to pass multiple scripts
    #.\RunAdHocScripts.ps1 -sqlServerName 'erp-r2-dev11' -databaseName 'Inquiry' -pathsToScripts ('C:\Projects\PCG\PCGInquiry\Inquiry\Scripts\Release.1.1\Navigation_Roles_PermissionContext.sql', 'C:\Projects\PCG\PCGInquiry\Inquiry\Scripts\Release.1.1\Test.sql')
foreach ($script in $scripts) {
    try {
        $path = "$pathToScripts\$script"
        Write-Host $path
        invoke-sqlcmd -inputfile $path -serverinstance $sqlServerName -database $databaseName
    } catch {
        Write-Host "Run Script ($script) Exception"
        $Error | format-list -force;
        Write-Host $Error[0].Exception.ParentContainsErrorRecordException
        #throw $Error[0].Exception.ParentContainsErrorRecordException;
    }
}
