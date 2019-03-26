
CREATE VIEW [dbo].[UserRoleView]
AS
	SELECT 
		ur.UserId,
		u.[Login],
		ur.RoleId,
		r.[Name] AS RoleName,
		ur.IsActive
	from [dbo].[User] u
		join [dbo].UserRole ur ON u.Id = ur.UserId 
		join [dbo].[Role] r ON ur.RoleId = r.Id
