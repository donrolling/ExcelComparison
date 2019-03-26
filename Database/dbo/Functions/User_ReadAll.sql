CREATE Function [dbo].[User_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[Id], [Login], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [User]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)