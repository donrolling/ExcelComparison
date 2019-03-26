CREATE Function [dbo].[User_SelectById] (@id bigint) RETURNS TABLE AS
	return 
		select top 1 
			[Id], [Login], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [User] 
		where Id = @id