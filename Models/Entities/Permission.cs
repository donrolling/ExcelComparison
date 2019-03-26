using Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public class Permission : Entity<long>
	{
		[StringLength(150, ErrorMessage = "Action cannot be longer than 150 characters.")]
		[Display(Name = "Action")]
		public string Action { get; set; }

		[Required]
		[StringLength(150, ErrorMessage = "Name cannot be longer than 150 characters.")]
		[Display(Name = "Name")]
		public string Name { get; set; }
	}

	public class Permission_Properties : Entity_Properties
	{
		public const string Action = "Action";
		public const string Id = "Id";
		public const string Name = "Name";
	}
}