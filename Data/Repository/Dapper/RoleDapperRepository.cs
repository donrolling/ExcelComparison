using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class RoleDapperRepository : RoleDapperBaseRepository, IRoleRepository
	{
		public RoleDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public async Task<IEnumerable<Role>> ReadAll()
		{
			var sql = @"SELECT * FROM [dbo].[Role]";
			return await this.QueryAsync<Role>(sql);
		}
	}
}