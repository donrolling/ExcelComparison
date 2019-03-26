
CREATE PROCEDURE [dbo].[RolePermission_DeleteByPermissionId]
	@permissionId bigint
AS
	DELETE FROM [dbo].[RolePermission]
	WHERE PermissionId = @permissionId