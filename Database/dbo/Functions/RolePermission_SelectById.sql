CREATE Function [dbo].[RolePermission_SelectById] (@roleId bigint, @permissionId bigint) RETURNS TABLE AS
	return 
		select top 1 
			[RoleId], [PermissionId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [RolePermission] 
		where RoleId = @roleId and PermissionId = @permissionId