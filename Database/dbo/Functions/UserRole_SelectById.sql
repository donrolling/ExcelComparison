CREATE Function [dbo].[UserRole_SelectById] (@userId bigint, @roleId bigint) RETURNS TABLE AS
	return 
		select top 1 
			[UserId], [RoleId], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [UserRole] 
		where UserId = @userId and RoleId = @roleId