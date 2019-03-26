using Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Models.Base {
	public abstract class BaseEntity<T> : IEntity<T> where T : struct {
		[Key]
		[Required]
		public virtual T Id { get; set; }
	}
}