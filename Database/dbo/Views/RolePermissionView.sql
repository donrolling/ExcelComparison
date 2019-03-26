
CREATE VIEW [dbo].[RolePermissionView]
AS
	SELECT 
		rp.RoleId,
		r.[Name] AS RoleName,
		rp.PermissionId, 
		p.[Name] AS PermissionName,
		p.[Action], 
		rp.IsActive
	from [dbo].Permission p
		join [dbo].RolePermission rp ON p.Id = rp.PermissionId 
		join [dbo].Role r ON rp.RoleId = r.Id
