using Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public class Role : Entity<long>
	{
		[Required]
		[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
		[Display(Name = "Name")]
		public string Name { get; set; }
	}

	public class Role_Properties : Entity_Properties
	{
		public const string Id = "Id";
		public const string Name = "Name";
	}
}