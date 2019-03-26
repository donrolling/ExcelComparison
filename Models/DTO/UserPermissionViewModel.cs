using System;

namespace Models.DTO {
	public class UserPermissionViewModel {
		public Guid Guid { get; set; }
		public string Action { get; set; }
		public long CreatedById { get; set; }
		public DateTime CreatedDate { get; set; }
		public long Id { get; set; }
		public bool IsActive { get; set; }
		public string Login { get; set; }
		public string NavigationSection { get; set; }
		public string RoleName { get; set; }
		public long UpdatedById { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}