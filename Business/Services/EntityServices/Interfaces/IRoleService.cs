using Data.Dapper.Models;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces
{
	public interface IRoleService
	{
		Task<InsertResponse<long>> Create(Role role);

		Task<InsertResponse<long>> Create(RoleDTO roleDTO);

		Task<TransactionResponse> Delete(long id);

		Task<TransactionResponse> Delete(RoleDTO roleDTO);

		Task<IEnumerable<Role>> ReadAll();

		Task<IDataResult<Role>> ReadAll(PageInfo pageInfo);

		Task<IDataResult<Role>> ReadAll(ClientPageInfo clientPageInfo);

		Task<TransactionResponse> Save(Role role);

		Task<Role> SelectById(long id);

		Task<RoleDTO> SelectRoleViewById(long roleId);

		Task<TransactionResponse> Update(RoleDTO roleDTO);

		Task<TransactionResponse> Update(Role role);
	}
}