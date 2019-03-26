
CREATE PROCEDURE [dbo].[UserRole_DeleteByUserId]
	@userId bigint
AS
	DELETE FROM [dbo].[UserRole]
	WHERE UserId = @userId