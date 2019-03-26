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
	public class RolePermissionDapperRepository : RolePermissionDapperBaseRepository, IRolePermissionRepository
	{
		public RolePermissionDapperRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public async Task<TransactionResponse> DeleteByPermissionId(long permissionId)
		{
			var sql = "Execute [dbo].[RolePermission_DeleteByPermissionId] @permissionId";
			var result = await base.ExecuteAsync(sql, new
			{
				permissionId = permissionId
			});
			return result;
		}

		public async Task<TransactionResponse> DeleteByRoleId(long roleId)
		{
			var sql = "Execute [dbo].[RolePermission_DeleteByRoleId] @roleId";
			var result = await base.ExecuteAsync(sql, new
			{
				roleId = roleId
			});
			return result;
		}

		public async Task<IEnumerable<RolePermissionDTO>> SelectByRoleId(long roleId)
		{
			var sql = @"SELECT * FROM [dbo].[RolePermissionView_SelectByRoleId](@roleId)";
			return await this.QueryAsync<RolePermissionDTO>(sql, new { roleId = roleId });
		}
	}
}