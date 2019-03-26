using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class PermissionDapperRepository : PermissionDapperBaseRepository, IPermissionRepository
	{
		public PermissionDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public async Task<IEnumerable<Permission>> ReadAll()
		{
			var sql = @"SELECT * FROM [dbo].[Permission]";
			return await this.QueryAsync<Permission>(sql);
		}
	}
}