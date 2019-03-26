CREATE PROCEDURE [dbo].[RolePermission_Delete]
	@roleId bigint, @permissionId bigint
AS
	DELETE FROM [dbo].[RolePermission]
	WHERE RoleId = @roleId and PermissionId = @permissionId