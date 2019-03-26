CREATE Function [dbo].[Role_SelectById] (@id bigint) RETURNS TABLE AS
	return 
		select top 1 
			[Id], [Name], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [Role] 
		where Id = @id