
CREATE PROCEDURE [dbo].[RolePermission_DeleteByRoleId]
	@roleId bigint
AS
	DELETE FROM [dbo].[RolePermission]
	WHERE RoleId = @roleId