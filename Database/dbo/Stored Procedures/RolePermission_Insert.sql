CREATE PROCEDURE [dbo].[RolePermission_Insert] (
	@roleId bigint,
	@permissionId bigint,
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime
) AS
	INSERT INTO [dbo].[RolePermission]	(
		[RoleId], [PermissionId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@roleId, @permissionId, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)