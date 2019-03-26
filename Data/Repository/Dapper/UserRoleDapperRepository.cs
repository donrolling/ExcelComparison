using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class UserRoleDapperRepository : UserRoleDapperBaseRepository, IUserRoleRepository
	{
		public UserRoleDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public async Task<TransactionResponse> DeleteByRoleId(long roleId)
		{
			var sql = "Execute [dbo].[UserRole_DeleteByRoleId] @roleId";
			var result = await base.ExecuteAsync(sql, new
			{
				roleId = roleId
			});
			return result;
		}

		public async Task<TransactionResponse> DeleteByUserId(long userId)
		{
			var sql = "Execute [dbo].[UserRole_DeleteByUserId] @userId";
			var result = await base.ExecuteAsync(sql, new
			{
				userId = userId
			});
			return result;
		}

		public async Task<IEnumerable<UserRoleDTO>> SelectByUserId(long userId)
		{
			var sql = @"SELECT * FROM [dbo].[UserRoleView_SelectByUserId](@userId)";
			return await this.QueryAsync<UserRoleDTO>(sql, new { userId = userId });
		}
	}
}