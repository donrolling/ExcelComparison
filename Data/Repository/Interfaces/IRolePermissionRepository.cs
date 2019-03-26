using Data.Dapper.Models;
using Models.DTO;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IRolePermissionRepository : IAssociativeDapperRepository<RolePermission, long, long>
	{
		Task<TransactionResponse> DeleteByPermissionId(long permissionId);

		Task<TransactionResponse> DeleteByRoleId(long roleId);

		Task<IEnumerable<RolePermissionDTO>> SelectByRoleId(long roleId);
	}
}