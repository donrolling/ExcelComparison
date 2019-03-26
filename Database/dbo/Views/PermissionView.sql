CREATE VIEW [dbo].[PermissionView]
AS
	select 
		u.Id as UserId,
		u.[Login],
		r.[Name] as RoleName,
		p.[Name] as NavigationSection, 
		p.[Action]
	from [dbo].[User] u 
		left join [dbo].UserRole ur ON ur.UserId  = u.Id
		left join [dbo].[Role] r ON r.Id = ur.RoleId
		left join [dbo].RolePermission rp ON rp.RoleId = r.Id 
		left join [dbo].Permission p ON p.Id = rp.PermissionId
GO