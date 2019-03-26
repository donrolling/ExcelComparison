CREATE PROCEDURE [dbo].[Permission_Update]
	@id bigint, @name nvarchar(150), @action nvarchar(150), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[Permission]
	SET	
		[Name] = @name,
		[Action] = @action,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id