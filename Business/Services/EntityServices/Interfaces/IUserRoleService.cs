using Data.Dapper.Models;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces
{
	public interface IUserRoleService
	{
		Task<TransactionResponse> Create(UserRole userRole);

		Task<TransactionResponse> Create(long userId, long[] roleIds);

		Task<TransactionResponse> Delete(long userId, long roleId);

		Task<TransactionResponse> DeleteByRoleId(long roleId);

		Task<TransactionResponse> DeleteByUserId(long userId);

		Task<IDataResult<UserRole>> ReadAll(PageInfo pageInfo);

		Task<IDataResult<UserRole>> ReadAll(ClientPageInfo clientPageInfo);

		Task<UserRole> SelectById(long userId, long roleId);

		Task<IEnumerable<UserRoleDTO>> SelectByUserId(long userId);

		Task<TransactionResponse> Update(long userId, long[] roleIds);
	}
}