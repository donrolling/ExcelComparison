using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services.EntityServices
{
	public class UserRoleService : UserRoleBaseService, IUserRoleService
	{
		public UserRoleService(IMembershipService membershipService, IUserRoleRepository userRoleRepository, ILoggerFactory loggerFactory) : base(membershipService, userRoleRepository, loggerFactory)
		{
		}

		public async Task<TransactionResponse> Create(long userId, long[] roleIds)
		{
			foreach (var roleId in roleIds) {
				var userRole = new UserRole { UserId = userId, RoleId = roleId };
				var result = await Create(userRole);
				if (result.Failure) {
					return result;
				}
			}
			return TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Success, StatusDetail.OK);
		}

		public async Task<TransactionResponse> DeleteByRoleId(long roleId)
		{
			return await UserRoleRepository.DeleteByRoleId(roleId);
		}

		public async Task<TransactionResponse> DeleteByUserId(long userId)
		{
			return await UserRoleRepository.DeleteByUserId(userId);
		}

		public async Task<IEnumerable<UserRoleDTO>> SelectByUserId(long userId)
		{
			return await UserRoleRepository.SelectByUserId(userId);
		}

		public async Task<TransactionResponse> Update(long userId, long[] roleIds)
		{
			var deleteResult = await DeleteByUserId(userId);
			if (deleteResult.Failure) {
				return deleteResult;
			}
			return await this.Create(userId, roleIds);
		}
	}
}