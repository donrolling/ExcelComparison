CREATE Function [dbo].[Permission_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[Id], [Name], [Action], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [Permission]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)