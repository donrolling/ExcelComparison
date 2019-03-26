CREATE PROCEDURE [dbo].[RolePermission_Update]
	@roleId bigint, @permissionId bigint, @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[RolePermission]
	SET	
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE RoleId = @roleId and PermissionId = @permissionId