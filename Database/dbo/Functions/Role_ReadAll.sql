CREATE Function [dbo].[Role_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[Id], [Name], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [Role]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)