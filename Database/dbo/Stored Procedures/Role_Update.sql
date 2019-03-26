CREATE PROCEDURE [dbo].[Role_Update]
	@id bigint, @name nvarchar(50), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[Role]
	SET	
		[Name] = @name,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id