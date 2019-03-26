namespace Models.DTO
{
    public class UserRoleDTO
    {
		public long UserId { get; set; }
		public string Login { get; set; }
		public long RoleId { get; set; }
		public string RoleName { get; set; }
		public bool IsActive { get; set; }
	}
}