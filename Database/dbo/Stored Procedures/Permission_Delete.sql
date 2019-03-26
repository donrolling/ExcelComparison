CREATE PROCEDURE [dbo].[Permission_Delete]
	@id bigint
AS
	DELETE FROM [dbo].[Permission]
	WHERE Id = @id