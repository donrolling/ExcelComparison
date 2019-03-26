CREATE Function [dbo].[RolePermission_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[RoleId], [PermissionId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [RolePermission]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)