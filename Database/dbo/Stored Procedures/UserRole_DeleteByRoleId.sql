
CREATE PROCEDURE [dbo].[UserRole_DeleteByRoleId]
	@roleId bigint
AS
	DELETE FROM [dbo].[UserRole]
	WHERE RoleId = @roleId