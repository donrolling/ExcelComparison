CREATE PROCEDURE [dbo].[UserRole_Delete]
	@userId bigint, @roleId bigint
AS
	DELETE FROM [dbo].[UserRole]
	WHERE UserId = @userId and RoleId = @roleId