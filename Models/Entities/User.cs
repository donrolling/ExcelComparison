using Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
	public class User : Entity<long>
	{
		[Required]
		[StringLength(150, ErrorMessage = "Login cannot be longer than 150 characters.")]
		[Display(Name = "Login")]
		public string Login { get; set; }
	}

	public class User_Properties : Entity_Properties
	{
		public const string Id = "Id";
		public const string Login = "Login";
	}
}