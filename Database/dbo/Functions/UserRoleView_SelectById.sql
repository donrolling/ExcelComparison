
CREATE Function [dbo].[UserRoleView_SelectById] (@userId bigint, @roleId bigint) RETURNS TABLE AS
	return 
		SELECT TOP 1 
			[UserId], [Login], [RoleId], [RoleName], [IsActive]	
		FROM [UserRoleView] v
		WHERE UserId = @userId and RoleId = @roleId