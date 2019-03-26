use [iAuditor]

declare @databaseName nvarchar(50) = 'iAuditor'
declare @sql nvarchar(max) = ''
declare @role nvarchar(50) = 'db_executor'
declare @svcAcct nvarchar(50) = 'PCGPORTAL\SVC-iAuditorDev'
declare @tfsAcct nvarchar(50) = 'PCGPORTAL\TFS-PCGIntegrationDe'
----------------------------------------------------------------------------------------------------
if not exists (select loginname from master.dbo.syslogins where name = @svcAcct)
begin
	print('login ' + @svcAcct + ' did not exist')
    select @sql = 'CREATE LOGIN [' + @svcAcct + '] FROM WINDOWS WITH DEFAULT_DATABASE = [' + @databaseName + '], DEFAULT_LANGUAGE = [us_english]'
	print(@sql)
    exec sp_executesql @sql
	print('login ' + @svcAcct + ' was created')
end
else begin
	print('login ' + @svcAcct + ' already existed')
end
print('')
if not exists(select * from sys.database_principals where [name] = @svcAcct)
begin
	print('user ' + @svcAcct + ' did not exist')
	select @sql = 'CREATE USER [' + @svcAcct + '] FOR LOGIN [' + @svcAcct + '] WITH DEFAULT_SCHEMA = dbo'
	print(@sql)
    exec sp_executesql @sql
	print('user ' + @svcAcct + ' was created')
end
else begin
	print('user ' + @svcAcct + ' already existed')
end
print('')
----------------------------------------------------------------------------------------------------
if not exists (select loginname from master.dbo.syslogins where name = @tfsAcct)
begin
	print('login ' + @tfsAcct + ' did not exist')
    select @sql = 'CREATE LOGIN [' + @tfsAcct + '] FROM WINDOWS WITH DEFAULT_DATABASE = [' + @databaseName + '], DEFAULT_LANGUAGE = [us_english]'
	print(@sql)
    exec sp_executesql @sql
	print('login ' + @tfsAcct + ' was created')
end
else begin
	print('login ' + @tfsAcct + ' already existed')
end
print('')
if not exists(select * from sys.database_principals where [name] = @tfsAcct)
begin
	print(@tfsAcct + ' did not exist')
	select @sql = 'CREATE USER [' + @tfsAcct + '] FOR LOGIN  [' + @tfsAcct + '] WITH DEFAULT_SCHEMA = dbo'
	print(@sql)
    exec sp_executesql @sql
	print('user ' + @tfsAcct + ' was created')
end
else begin
	print('user ' + @tfsAcct + ' already existed')
end
----------------------------------------------------------------------------------------------------
print('')
iF DATABASE_PRINCIPAL_ID(@role) IS NULL begin
	print('role ' + @role + ' did not exist')	
	select @sql = 'CREATE ROLE ' + QUOTENAME(@role) 
	print(@sql)
    exec sp_executesql @sql	
	print('role ' + @role + ' was created')
	select @sql = 'GRANT EXECUTE TO ' + QUOTENAME(@role) 
	print(@sql)
    exec sp_executesql @sql
	print('execute was granted to ' + @role)
end
else begin
	print('role ' + @role + ' already existed')
	select @sql = 'GRANT EXECUTE TO ' + QUOTENAME(@role) 
	print(@sql)
    exec sp_executesql @sql
	print('execute was granted to ' + @role)
end
----------------------------------------------------------------------------------------------------
print('')
if(isnull(IS_ROLEMEMBER(@role, @svcAcct), 0) = 0) begin
    print(@svcAcct + ' is not in ' + @role)
	exec sp_addrolemember @role, @svcAcct 
    print(@svcAcct + ' was added to ' + @role)
end
else begin
    print(@svcAcct + ' is already in ' + @role)
end
----------------------------------------------------------------------------------------------------
print('')
if(isnull(IS_ROLEMEMBER('db_datareader', @svcAcct), 0) = 0) begin
    print(@svcAcct + ' is not in db_datareader')
	exec sp_addrolemember 'db_datareader', @svcAcct 
    print(@svcAcct + ' was added to db_datareader')
end
else begin
    print(@svcAcct + ' is already in ' + @role)
end
----------------------------------------------------------------------------------------------------
print('')
if(isnull(IS_ROLEMEMBER('db_owner', @tfsAcct), 0) = 0) begin
    print(@tfsAcct + ' is not in db_owner')
	exec sp_addrolemember 'db_owner', @tfsAcct 
    print(@tfsAcct + ' was added to db_owner')
end
else begin
    print(@tfsAcct + ' is already in db_owner')
end
----------------------------------------------------------------------------------------------------
USE [MicrosoftDynamicsAX]
print('')
if not exists(select * from sys.database_principals where [name] = @svcAcct)
begin
	print(@svcAcct + ' did not exist')
	select @sql = 'CREATE USER [' + @svcAcct + '] FOR LOGIN  [' + @svcAcct + '] WITH DEFAULT_SCHEMA = dbo'
	print(@sql)
    exec sp_executesql @sql
	print('user ' + @svcAcct + ' was created')
end
else begin
	print('user ' + @svcAcct + ' already existed')
end
print('')
if(isnull(IS_ROLEMEMBER('db_datareader', @svcAcct), 0) = 0) begin
    print(@svcAcct + ' is not in db_datareader')
	exec sp_addrolemember 'db_datareader', @svcAcct 
    print(@svcAcct + ' was added to db_datareader')
end
else begin
    print(@svcAcct + ' is already in db_datareader')
end

