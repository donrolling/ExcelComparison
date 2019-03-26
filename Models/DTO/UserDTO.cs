using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.DTO
{
    public class UserDTO
    {
		[Key]
		public long Id { get; set; }
		
		[Required]
		[StringLength(150, ErrorMessage = "Login cannot be longer than 150 characters.")]
		[Display(Name = "Login")]
		public string Login { get; set; }
		
		[Display(Name = "Created By Id")]
		public long CreatedById { get; set; }
		
		[Display(Name = "Created Date")]
		public DateTime CreatedDate { get; set; }
		
		[Required]
		[Display(Name = "Is Active")]
		public bool IsActive { get; set; } = true;
		
		[Display(Name = "Updated By Id")]
		public long UpdatedById { get; set; }
		
		[Display(Name = "Updated Date")]
		public DateTime UpdatedDate { get; set; }

		[Display(Name = "Roles")]
		public List<UserRoleDTO> Roles { get; set; }

		public long[] RoleIds { get; set; }
	}
}