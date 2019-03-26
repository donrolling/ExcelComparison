namespace Models.DTO
{
    public class RolePermissionDTO
    {
		public long RoleId { get; set; }
		public string RoleName { get; set; }
		public long PermissionId { get; set; }
		public string PermissionName { get; set; }
		public string Action { get; set; }
		public bool IsActive { get; set; }

		public string SelectListItemDisplay
		{
			get
			{
				return $"{PermissionName} - {Action}";
			}
		}
	}
}