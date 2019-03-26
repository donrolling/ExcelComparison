using Data.Interfaces;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IPermissionRepository : IEntityDapperRepository<Permission, long>
	{
		Task<IEnumerable<Permission>> ReadAll();
	}
}