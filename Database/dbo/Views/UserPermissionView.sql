CREATE VIEW [dbo].UserPermissionView
AS
SELECT
	u.Id
	, u.[Login]
	, u.IsActive
	, u.CreatedById
	, u.CreatedDate
	, u.UpdatedById
	, u.UpdatedDate
	, r.[Name] as RoleName
	, p.[Name] as NavigationSection
	, p.[Action]
FROM [dbo].[User] AS u 
	left join [dbo].UserRole AS ur ON ur.UserId = u.Id
	left join [dbo].[Role] AS r ON r.Id = ur.RoleId
	left join [dbo].RolePermission AS rp ON rp.PermissionId = r.Id
	left join [dbo].Permission AS p ON p.Id = rp.PermissionId
