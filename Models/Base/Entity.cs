using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Base {
	public abstract class Entity<T> : BaseEntity<T> where T : struct {
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
	}
}