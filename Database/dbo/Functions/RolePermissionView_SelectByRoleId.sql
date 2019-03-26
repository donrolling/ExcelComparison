
CREATE Function [dbo].[RolePermissionView_SelectByRoleId] (@roleId bigint) RETURNS TABLE AS
	return 
		SELECT 
			[RoleId], [RoleName], [PermissionId], [PermissionName], [Action], [IsActive]
		FROM [dbo].[RolePermissionView] 
		WHERE RoleId = @roleId