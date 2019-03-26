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
	public class UserRoleBaseService : EntityServiceBase
	{
		public IMembershipService MembershipService { get; set; }
		public IUserRoleRepository UserRoleRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		public UserRoleBaseService(IMembershipService membershipService, IUserRoleRepository userRoleRepository, ILoggerFactory loggerFactory) : base(_auditing, loggerFactory)
		{
			this.MembershipService = membershipService;
			this.UserRoleRepository = userRoleRepository;
		}

		public virtual async Task<TransactionResponse> Create(UserRole userRole)
		{
			var prepareForSaveResult = await base.PrepareForSave_Async<UserRole, long, long>(userRole, await this.MembershipService.CurrentUserId());
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.UserRoleRepository.Create(userRole);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long userId, long roleId)
		{
			var result = await this.UserRoleRepository.Delete(userId, roleId);
			return result;
		}

		public virtual async Task<IDataResult<UserRole>> ReadAll(PageInfo pageInfo)
		{
			return await this.UserRoleRepository.ReadAll(pageInfo);
		}

		public virtual async Task<IDataResult<UserRole>> ReadAll(ClientPageInfo clientPageInfo)
		{
			var pageInfo = ClientPageInfo.ConvertToPageInfo(clientPageInfo);
			return await this.ReadAll(pageInfo);
		}

		public virtual async Task<UserRole> SelectById(long userId, long roleId)
		{
			return await this.UserRoleRepository.SelectById(userId, roleId);
		}
	}
}