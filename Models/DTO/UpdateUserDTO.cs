using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
	public class UpdateUserDTO : User
	{
		public IEnumerable<SelectListItem> AllRoles { get; set; }

		public IEnumerable<SelectListItem> AvailableRoles { get; set; }

		public long[] RoleIds { get; set; }

		[Display(Name = "Roles")]
		public IEnumerable<UserRoleDTO> Roles { get; set; }

		public IEnumerable<SelectListItem> SelectedRoles { get; set; }
	}
}