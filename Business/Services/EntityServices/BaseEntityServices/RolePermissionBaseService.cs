using Business.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Base;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Business.Services.EntityServices.BaseServices
{
	public class RolePermissionBaseService : EntityServiceBase
	{
		public IMembershipService MembershipService { get; set; }
		public IRolePermissionRepository RolePermissionRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public RolePermissionBaseService(IMembershipService membershipService, IRolePermissionRepository rolePermissionRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory)
		{
			this.MembershipService = membershipService;
			this.RolePermissionRepository = rolePermissionRepository;
		}

		public virtual async Task<TransactionResponse> Create(RolePermission rolePermission)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<RolePermission, long, long>(rolePermission, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.RolePermissionRepository.Create(rolePermission);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long roleId, long permissionId)
		{
			var result = await this.RolePermissionRepository.Delete(roleId, permissionId);
			return result;
		}

		public virtual async Task<IDataResult<RolePermission>> ReadAll(PageInfo pageInfo)
		{
			return await this.RolePermissionRepository.ReadAll(pageInfo);
		}

		public virtual async Task<IDataResult<RolePermission>> ReadAll(ClientPageInfo clientPageInfo)
		{
			var pageInfo = ClientPageInfo.ConvertToPageInfo(clientPageInfo);
			return await this.ReadAll(pageInfo);
		}

		public virtual async Task<RolePermission> SelectById(long roleId, long permissionId)
		{
			return await this.RolePermissionRepository.SelectById(roleId, permissionId);
		}
	}
}