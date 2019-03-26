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
	public class PermissionBaseService : EntityServiceBase
	{
		public IMembershipService MembershipService { get; set; }
		public IPermissionRepository PermissionRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public PermissionBaseService(IMembershipService membershipService, IPermissionRepository permissionRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory)
		{
			this.MembershipService = membershipService;
			this.PermissionRepository = permissionRepository;
		}

		public virtual async Task<InsertResponse<long>> Create(Permission permission)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<Permission, long>(permission, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.PermissionRepository.Create(permission);
			permission.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			var result = await this.PermissionRepository.Delete(id);
			return result;
		}

		public virtual async Task<IDataResult<Permission>> ReadAll(PageInfo pageInfo)
		{
			return await this.PermissionRepository.ReadAll(pageInfo);
		}

		public virtual async Task<IDataResult<Permission>> ReadAll(ClientPageInfo clientPageInfo)
		{
			var pageInfo = ClientPageInfo.ConvertToPageInfo(clientPageInfo);
			return await this.ReadAll(pageInfo);
		}

		public virtual async Task<TransactionResponse> Save(Permission permission)
		{
			if (permission.Id == 0) {
				return await this.Create(permission);
			} else {
				return await this.Update(permission);
			}
		}

		public virtual async Task<Permission> SelectById(long id)
		{
			return await this.PermissionRepository.SelectById(id);
		}

		public virtual async Task<TransactionResponse> Update(Permission permission)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<Permission, long>(permission, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.PermissionRepository.Update(permission);
			return result;
		}
	}
}