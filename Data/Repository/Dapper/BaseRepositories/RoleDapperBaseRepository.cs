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
	public class RoleDapperBaseRepository : DapperAsyncRepository, IEntityDapperRepository<Role, long>
	{
		public RoleDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public RoleDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory)
		{
		}

		public virtual async Task<InsertResponse<long>> Create(Role role)
		{
			var sql = "Execute [dbo].[Role_Insert] @Name, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate, @Id OUTPUT";
			var _params = new DynamicParameters();
			_params.Add("Name", role.Name);
			_params.Add("IsActive", role.IsActive);
			_params.Add("CreatedById", role.CreatedById);
			_params.Add("CreatedDate", role.CreatedDate);
			_params.Add("UpdatedById", role.UpdatedById);
			_params.Add("UpdatedDate", role.UpdatedDate);
			_params.Add("Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
			var result = await base.ExecuteAsync(sql, _params);
			return InsertResponse<long>.GetInsertResponse(result);
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			var sql = "Execute [dbo].[Role_Delete] @id";
			var result = await base.ExecuteAsync(sql, new
			{
				id = id,
			});
			return result;
		}

		public virtual async Task<IDataResult<Role>> ReadAll(PageInfo pageInfo)
		{
			return await this.QueryAsync<Role>(new Role_ReadAll_Function(), pageInfo);
		}

		public virtual async Task<Role> SelectById(long id)
		{
			return await this.QuerySingleAsync<Role>(new Role_SelectById_Function(id));
		}

		public virtual async Task<TransactionResponse> Update(Role role)
		{
			var sql = "Execute [dbo].[Role_Update] @Id, @Name, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, role);
			return result;
		}
	}
}