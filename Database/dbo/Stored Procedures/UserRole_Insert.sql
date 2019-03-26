CREATE PROCEDURE [dbo].[UserRole_Insert] (
	@userId bigint,
	@roleId bigint,
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime
) AS
	INSERT INTO [dbo].[UserRole]	(
		[UserId], [RoleId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@userId, @roleId, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)