using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class UserDapperRepository : UserDapperBaseRepository, IUserRepository
	{
		public UserDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public async Task<IEnumerable<PermissionViewModel>> GetUserClaims_ByLogin(string login)
		{
			var sql = @"select *
						from [dbo].[PermissionView]
						where [Login] = @login";
			return await this.QueryAsync<PermissionViewModel>(sql, new { login = login });
		}

		public async Task<IEnumerable<PermissionViewModel>> GetUserClaims_ByUserId(long id)
		{
			var sql = @"select *
						from [dbo].[PermissionView]
						where UserId = @id";
			return await this.QueryAsync<PermissionViewModel>(sql, new { id = id });
		}
	}
}