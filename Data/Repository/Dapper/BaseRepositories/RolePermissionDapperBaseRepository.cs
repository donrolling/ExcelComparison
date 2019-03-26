using Data.Dapper.Models;
using Data.Repository.FunctionDefinitions;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class RolePermissionDapperBaseRepository : DapperAsyncRepository, IAssociativeDapperRepository<RolePermission, long, long>
	{
		public RolePermissionDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public RolePermissionDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory)
		{
		}

		public virtual async Task<TransactionResponse> Create(RolePermission rolePermission)
		{
			var sql = "Execute [dbo].[RolePermission_Insert] @RoleId, @PermissionId, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, rolePermission);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long roleId, long permissionId)
		{
			var sql = "Execute [dbo].[RolePermission_Delete] @roleId, @permissionId";
			var result = await base.ExecuteAsync(sql, new
			{
				roleId = roleId,
				permissionId = permissionId,
			});
			return result;
		}

		public virtual async Task<IDataResult<RolePermission>> ReadAll(PageInfo pageInfo)
		{
			return await this.QueryAsync<RolePermission>(new RolePermission_ReadAll_Function(), pageInfo);
		}

		public virtual async Task<RolePermission> SelectById(long roleId, long permissionId)
		{
			return await this.QuerySingleAsync<RolePermission>(new RolePermission_SelectById_Function(roleId, permissionId));
		}

		public virtual async Task<TransactionResponse> Update(RolePermission rolePermission)
		{
			var sql = "Execute [dbo].[RolePermission_Update] @RoleId, @PermissionId, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, rolePermission);
			return result;
		}
	}
}