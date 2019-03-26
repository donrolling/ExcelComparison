CREATE PROCEDURE [dbo].[User_Delete]
	@id bigint
AS
	DELETE FROM [dbo].[User]
	WHERE Id = @id