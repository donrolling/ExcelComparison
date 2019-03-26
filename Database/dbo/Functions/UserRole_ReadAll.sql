CREATE Function [dbo].[UserRole_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[UserId], [RoleId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [UserRole]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)