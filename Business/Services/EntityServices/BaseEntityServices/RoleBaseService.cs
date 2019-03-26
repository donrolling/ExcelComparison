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
	public class RoleBaseService : EntityServiceBase
	{
		public IMembershipService MembershipService { get; set; }
		public IRoleRepository RoleRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public RoleBaseService(IMembershipService membershipService, IRoleRepository roleRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory)
		{
			this.MembershipService = membershipService;
			this.RoleRepository = roleRepository;
		}

		public virtual async Task<InsertResponse<long>> Create(Role role)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<Role, long>(role, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.RoleRepository.Create(role);
			role.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			var result = await this.RoleRepository.Delete(id);
			return result;
		}

		public virtual async Task<IDataResult<Role>> ReadAll(PageInfo pageInfo)
		{
			return await this.RoleRepository.ReadAll(pageInfo);
		}

		public virtual async Task<IDataResult<Role>> ReadAll(ClientPageInfo clientPageInfo)
		{
			var pageInfo = ClientPageInfo.ConvertToPageInfo(clientPageInfo);
			return await this.ReadAll(pageInfo);
		}

		public virtual async Task<TransactionResponse> Save(Role role)
		{
			if (role.Id == 0) {
				return await this.Create(role);
			} else {
				return await this.Update(role);
			}
		}

		public virtual async Task<Role> SelectById(long id)
		{
			return await this.RoleRepository.SelectById(id);
		}

		public virtual async Task<TransactionResponse> Update(Role role)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<Role, long>(role, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.RoleRepository.Update(role);
			return result;
		}
	}
}