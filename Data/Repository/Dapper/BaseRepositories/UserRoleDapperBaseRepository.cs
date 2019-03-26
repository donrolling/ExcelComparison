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
	public class UserRoleDapperBaseRepository : DapperAsyncRepository, IAssociativeDapperRepository<UserRole, long, long>
	{
		public UserRoleDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public UserRoleDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory)
		{
		}

		public virtual async Task<TransactionResponse> Create(UserRole userRole)
		{
			var sql = "Execute [dbo].[UserRole_Insert] @UserId, @RoleId, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, userRole);
			return result;
		}

		public virtual async Task<TransactionResponse> Delete(long userId, long roleId)
		{
			var sql = "Execute [dbo].[UserRole_Delete] @userId, @roleId";
			var result = await base.ExecuteAsync(sql, new
			{
				userId = userId,
				roleId = roleId,
			});
			return result;
		}

		public virtual async Task<IDataResult<UserRole>> ReadAll(PageInfo pageInfo)
		{
			return await this.QueryAsync<UserRole>(new UserRole_ReadAll_Function(), pageInfo);
		}

		public virtual async Task<UserRole> SelectById(long userId, long roleId)
		{
			return await this.QuerySingleAsync<UserRole>(new UserRole_SelectById_Function(userId, roleId));
		}

		public virtual async Task<TransactionResponse> Update(UserRole userRole)
		{
			var sql = "Execute [dbo].[UserRole_Update] @UserId, @RoleId, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, userRole);
			return result;
		}
	}
}