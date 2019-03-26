using Dapper;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Repository.FunctionDefinitions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Entities;
using Models.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class PermissionDapperBaseRepository : DapperAsyncRepository, IEntityDapperRepository<Permission, long>
	{
		public PermissionDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public PermissionDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory)
		{
		}

		public virtual async Task<InsertResponse<long>> Create(Permission permission)
		{
			var sql = "Execute [dbo].[Permission_Insert] @Name, @Action, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate, @Id OUTPUT";
			var _params = new DynamicParameters();
			_params.Add("Name", permission.Name);
			_params.Add("Action", permission.Action);
			_params.Add("IsActive", permission.IsActive);
			_params.Add("CreatedById", permission.CreatedById);
			_params.Add("CreatedDate", permission.CreatedDate);
			_params.Add("UpdatedById", permission.UpdatedById);
			_params.Add("UpdatedDate", permission.UpdatedDate);
			_params.Add("Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
			var result = await base.ExecuteAsync(sql, _params);
			return InsertResponse<long>.GetInsertResponse(result);
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			var sql = "Execute [dbo].[Permission_Delete] @id";
			var result = await base.ExecuteAsync(sql, new
			{
				id = id,
			});
			return result;
		}

		public virtual async Task<IDataResult<Permission>> ReadAll(PageInfo pageInfo)
		{
			return await this.QueryAsync<Permission>(new Permission_ReadAll_Function(), pageInfo);
		}

		public virtual async Task<Permission> SelectById(long id)
		{
			return await this.QuerySingleAsync<Permission>(new Permission_SelectById_Function(id));
		}

		public virtual async Task<TransactionResponse> Update(Permission permission)
		{
			var sql = "Execute [dbo].[Permission_Update] @Id, @Name, @Action, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, permission);
			return result;
		}
	}
}