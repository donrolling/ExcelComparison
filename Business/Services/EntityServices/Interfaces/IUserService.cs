using Data.Dapper.Models;
using Models.Application;
using Models.Entities;
using Models.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces
{
	public interface IUserService
	{
		Task<InsertResponse<long>> Create(User user, long currentUserId);

		Task<TransactionResponse> Delete(long id);

		Task<IEnumerable<Claim>> GetUserClaims(long id);

		Task<UserContext> GetUserContextByLogin(string name);

		Task<IDataResult<User>> ReadAll(PageInfo pageInfo);

		Task<IDataResult<User>> ReadAll(ClientPageInfo clientPageInfo);

		Task<TransactionResponse> Save(User user, long currentUserId);

		Task<User> SelectById(long id);

		Task<User> SelectByLogin(string name);

		Task<TransactionResponse> Update(User user, long currentUserId);
	}
}