using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.Base;
using Common.BaseClasses;
using Common.Transactions;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Application;
using Models.Base;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Services.EntityServices
{
	public class UserService : LoggingWorker, IUserService
	{
		public IUserRepository UserRepository { get; set; }

		private static readonly Auditing _auditing = new Auditing();

		private EntityServiceBase _entityServiceBase;

		public UserService(IUserRepository userRepository, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			this.UserRepository = userRepository;
			this._entityServiceBase = new EntityServiceBase(_auditing, loggerFactory);
		}

		public async Task<InsertResponse<long>> Create(User user, long currentUserId)
		{
			var prepareForSaveResult = await this._entityServiceBase.PrepareForSave_Async<User, long>(user, currentUserId);
			if (!prepareForSaveResult.IsValid) {
				return InsertResponse<long>.GetInsertResponse(TransactionResponse.GetTransactionResponse(ActionType.Create, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage));
			}
			var result = await this.UserRepository.Create(user);
			user.Id = result.Id;
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			return await this.UserRepository.Delete(id);
		}

		public async Task<IEnumerable<Claim>> GetUserClaims(long id)
		{
			var results = await this.UserRepository.GetUserClaims_ByUserId(id);
			var claims = new List<Claim>();
			foreach (var x in results) {
				claims.Add(new Claim(x.Action, x.NavigationSection));
			}
			return claims;
		}

		public async Task<UserContext> GetUserContextByLogin(string name)
		{
			var results = await this.UserRepository.GetUserClaims_ByLogin(name);
			if (results == null || !results.Any()) {
				return null;
			}

			var user = results.First();
			return new UserContext
			{
				Id = user.UserId,
				Login = user.Login,
				Claims = results
						.Where(a =>
							!string.IsNullOrEmpty(a.Action)
							&& !string.IsNullOrEmpty(a.NavigationSection)
						)
						.Select(a =>
							new MembershipClaim(a.NavigationSection, a.Action)
						)
			};
		}

		public async Task<IDataResult<User>> ReadAll(PageInfo pageInfo)
		{
			return await this.UserRepository.ReadAll(pageInfo);
		}

		public async Task<IDataResult<User>> ReadAll(ClientPageInfo clientPageInfo)
		{
			var pageInfo = ClientPageInfo.ConvertToPageInfo(clientPageInfo);
			return await this.ReadAll(pageInfo);
		}

		public async Task<TransactionResponse> Save(User user, long currentUserId)
		{
			if (user.Id == 0) {
				return await this.Create(user, currentUserId);
			} else {
				return await this.Update(user, currentUserId);
			}
		}

		public async Task<User> SelectById(long id)
		{
			return await this.UserRepository.SelectById(id);
		}

		public async Task<User> SelectByLogin(string name)
		{
			var pageInfo = new PageInfo(1);
			pageInfo.Filters.Add(new SearchFilter(User_Properties.Login, name));
			var userResult = await this.UserRepository.ReadAll(pageInfo);
			if (userResult.Total == 0) {
				return null;
			}
			return userResult.Data.First();
		}

		public async Task<TransactionResponse> Update(User user, long currentUserId)
		{
			var prepareForSaveResult = await this._entityServiceBase.PrepareForSave_Async<User, long>(user, currentUserId);
			if (!prepareForSaveResult.IsValid) {
				return TransactionResponse.GetTransactionResponse(ActionType.Update, Status.Failure, StatusDetail.Invalid, prepareForSaveResult.ValidationMessage);
			}
			var result = await this.UserRepository.Update(user);
			return result;
		}
	}
}