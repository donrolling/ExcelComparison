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
	public class UserDapperBaseRepository : DapperAsyncRepository, IEntityDapperRepository<User, long>
	{
		public UserDapperBaseRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(appSettings, loggerFactory)
		{
		}

		public UserDapperBaseRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(connectionString, appSettings, loggerFactory)
		{
		}

		public virtual async Task<InsertResponse<long>> Create(User user)
		{
			var sql = "Execute [dbo].[User_Insert] @Login, @IsActive, @CreatedById, @CreatedDate, @UpdatedById, @UpdatedDate, @Id OUTPUT";
			var _params = new DynamicParameters();
			_params.Add("Login", user.Login);
			_params.Add("IsActive", user.IsActive);
			_params.Add("CreatedById", user.CreatedById);
			_params.Add("CreatedDate", user.CreatedDate);
			_params.Add("UpdatedById", user.UpdatedById);
			_params.Add("UpdatedDate", user.UpdatedDate);
			_params.Add("Id", dbType: DbType.Int64, direction: ParameterDirection.Output);
			var result = await base.ExecuteAsync(sql, _params);
			return InsertResponse<long>.GetInsertResponse(result);
		}

		public virtual async Task<TransactionResponse> Delete(long id)
		{
			var sql = "Execute [dbo].[User_Delete] @id";
			var result = await base.ExecuteAsync(sql, new
			{
				id = id,
			});
			return result;
		}

		public virtual async Task<IDataResult<User>> ReadAll(PageInfo pageInfo)
		{
			return await this.QueryAsync<User>(new User_ReadAll_Function(), pageInfo);
		}

		public virtual async Task<User> SelectById(long id)
		{
			return await this.QuerySingleAsync<User>(new User_SelectById_Function(id));
		}

		public virtual async Task<TransactionResponse> Update(User user)
		{
			var sql = "Execute [dbo].[User_Update] @Id, @Login, @IsActive, @UpdatedById, @UpdatedDate";
			var result = await base.ExecuteAsync(sql, user);
			return result;
		}
	}
}