using Data.Interfaces;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IRoleRepository : IEntityDapperRepository<Role, long>
	{
		Task<IEnumerable<Role>> ReadAll();
	}
}