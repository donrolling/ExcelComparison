CREATE Function [dbo].[Permission_SelectById] (@id bigint) RETURNS TABLE AS
	return 
		select top 1 
			[Id], [Name], [Action], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [Permission] 
		where Id = @id