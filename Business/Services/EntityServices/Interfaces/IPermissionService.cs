using Data.Dapper.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces
{
	public interface IPermissionService
	{
		Task<InsertResponse<long>> Create(Permission permission);

		Task<TransactionResponse> Delete(long id);

		Task<TransactionResponse> Delete(Permission permission);

		Task<RoleDTO> GetEmptyRoleView();

		IEnumerable<SelectListItem> GetPermissionList(IEnumerable<Permission> allPermissions, IEnumerable<RolePermissionDTO> rolePermissions = null);

		Task<IEnumerable<Permission>> ReadAll();

		Task<IDataResult<Permission>> ReadAll(PageInfo pageInfo);

		Task<IDataResult<Permission>> ReadAll(ClientPageInfo clientPageInfo);

		Task<TransactionResponse> Save(Permission permission);

		Task<Permission> SelectById(long id);

		Task<TransactionResponse> Update(Permission permission);
	}
}