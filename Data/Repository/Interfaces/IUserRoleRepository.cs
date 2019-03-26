using Data.Dapper.Models;
using Models.DTO;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IUserRoleRepository : IAssociativeDapperRepository<UserRole, long, long>
	{
		Task<TransactionResponse> DeleteByRoleId(long roleId);

		Task<TransactionResponse> DeleteByUserId(long userId);

		Task<IEnumerable<UserRoleDTO>> SelectByUserId(long userId);
	}
}