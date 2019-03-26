
CREATE Function [dbo].[UserRoleView_SelectByUserId] (@userId bigint) RETURNS TABLE AS
	return 
		SELECT 
			[UserId], [Login], [RoleId], [RoleName], [IsActive]	
		FROM [UserRoleView] v
		WHERE UserId = @userId