using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.Collections.Generic;

namespace Models.DTO
{
	public class RoleDTO : Role
	{
		public IEnumerable<SelectListItem> AllPermissions { get; set; } = new List<SelectListItem>();

		public IEnumerable<SelectListItem> AvailablePermissions { get; set; } = new List<SelectListItem>();

		public long[] PermissionIds { get; set; }

		public IEnumerable<RolePermissionDTO> Permissions { get; set; } = new List<RolePermissionDTO>();

		public IEnumerable<SelectListItem> SelectedPermissions { get; set; } = new List<SelectListItem>();
	}
}