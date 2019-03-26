using Business.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Business.Service.EntityServices.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dapper.Models;
using Models.Entities;
using Common.Transactions;

namespace Business.Services.EntityServices
{
	public class RolePermissionService : RolePermissionBaseService, IRolePermissionService
	{

		public RolePermissionService(IMembershipService membershipService, IRolePermissionRepository rolePermissionRepository, ILoggerFactory loggerFactory) : base(membershipService, rolePermissionRepository, loggerFactory) { }

		public async Task<IEnumerable<RolePermissionDTO>> SelectByRoleId(long roleId)
		{
			return await RolePermissionRepository.SelectByRoleId(roleId);
		}

		public async Task<TransactionResponse> Create(long roleId, long[] permissionIds)
		{
			foreach (var permissionId in permissionIds)
			{
				var x = new RolePermission { RoleId = roleId, PermissionId = permissionId, IsActive = true };
				var result = await Create(x);
				if (result.Failure) {
					return result;
				}
			}
			return TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Success, StatusDetail.OK);
		}

		public async Task<TransactionResponse> Update(long roleId, long[] permissionIds)
		{
			if (permissionIds == null) {
				permissionIds = new long[0];
			}
			var deleteResult = await this.DeleteByRoleId(roleId);
			if (deleteResult.Failure) {
				return deleteResult;
			}
			foreach (var permissionId in permissionIds)
			{
				var x = new RolePermission { RoleId = roleId, PermissionId = permissionId };
				var result = await Create(x);
				if (result.Failure) {
					return result;
				}
			}
			return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Success, StatusDetail.OK);
		}

		public async Task<TransactionResponse> DeleteByRoleId(long roleId)
		{
			return await RolePermissionRepository.DeleteByRoleId(roleId);
		}

		public async Task<TransactionResponse> DeleteByPermissionId(long permissionId)
		{
			return await RolePermissionRepository.DeleteByPermissionId(permissionId);
		}
	}
}