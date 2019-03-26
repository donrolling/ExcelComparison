using Data.Interfaces;
using Models.DTO;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IUserRepository : IEntityDapperRepository<User, long>
	{
		Task<IEnumerable<PermissionViewModel>> GetUserClaims_ByLogin(string name);

		Task<IEnumerable<PermissionViewModel>> GetUserClaims_ByUserId(long id);
	}
}