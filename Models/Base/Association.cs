using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Base {
	public abstract class Association<T, U> : IAssociation<T, U> where T : struct {
		[Display(Name = "Created By Id")]
		public long CreatedById { get; set; }
		[Display(Name = "Created Date")]
		public DateTime CreatedDate { get; set; }
		[Key]
		[Required]
		public virtual T FK_One { get; set; }
		[Key]
		[Required]
		public virtual U FK_Two { get; set; }
		[Required]
		[Display(Name = "Is Active")]
		public bool IsActive { get; set; } = true;
		[Display(Name = "Updated By Id")]
		public long UpdatedById { get; set; }
		[Display(Name = "Updated Date")]
		public DateTime UpdatedDate { get; set; }
	}
}