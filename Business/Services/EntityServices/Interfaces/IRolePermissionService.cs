using Data.Dapper.Models;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces {
	public interface IRolePermissionService {
		Task<TransactionResponse> Create(RolePermission rolePermission);
		Task<TransactionResponse> Create(long roleId, long[] permissionIds);
		Task<TransactionResponse> Update(long roleId, long[] permissionIds);
		Task<TransactionResponse> Delete(long roleId, long permissionId);
		Task<TransactionResponse> DeleteByRoleId(long roleId);
		Task<TransactionResponse> DeleteByPermissionId(long permissionId);
		Task<IDataResult<RolePermission>> ReadAll(PageInfo pageInfo);
		Task<IDataResult<RolePermission>> ReadAll(ClientPageInfo clientPageInfo);
		Task<RolePermission> SelectById(long roleId, long permissionId);
		Task<IEnumerable<RolePermissionDTO>> SelectByRoleId(long roleId);
	}
}