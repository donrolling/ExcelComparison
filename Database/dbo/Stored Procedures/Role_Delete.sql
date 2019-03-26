CREATE PROCEDURE [dbo].[Role_Delete]
	@id bigint
AS
	DELETE FROM [dbo].[Role]
	WHERE Id = @id