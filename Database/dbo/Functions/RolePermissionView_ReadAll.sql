
CREATE Function [dbo].[RolePermissionView_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	SELECT 
		[RoleId], [RoleName], [PermissionId], [PermissionName], [Action], [IsActive] 
	FROM [dbo].[RolePermissionView]
	WHERE 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)