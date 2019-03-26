CREATE PROCEDURE [dbo].[UserRole_Update]
	@userId bigint, @roleId bigint, @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[UserRole]
	SET	
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE UserId = @userId and RoleId = @roleId