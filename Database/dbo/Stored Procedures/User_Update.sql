CREATE PROCEDURE [dbo].[User_Update]
	@id bigint, @login nvarchar(150), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[User]
	SET	
		[Login] = @login,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id