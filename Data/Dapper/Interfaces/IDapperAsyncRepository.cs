using Dapper;
using Data.Dapper.Models;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Data.Dapper.Interfaces
{
	public interface IDapperAsyncRepository : IAsyncRepository
	{
		ILogger Logger { get; }

		TransactionResponse Execute(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);

		TransactionResponse Execute(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);

		Task<TransactionResponse> ExecuteAsync(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);

		IEnumerable<object> Query(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		IEnumerable<R> Query<T, U, R>(string sql, System.Func<T, U, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class;

		IEnumerable<R> Query<T, U, V, R>(string sql, System.Func<T, U, V, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where R : class;

		IEnumerable<R> Query<T, U, V, W, R>(string sql, System.Func<T, U, V, W, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class;

		IEnumerable<T> Query<T>(IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		IDataResult<T> Query<T>(IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		IEnumerable<T> Query<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		IDataResult<T> Query<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<IEnumerable<object>> QueryAsync(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		Task<IEnumerable<R>> QueryAsync<T, U, R>(string sql, System.Func<T, U, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class;

		Task<IEnumerable<R>> QueryAsync<T, U, V, R>(string sql, System.Func<T, U, V, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where R : class;

		Task<IEnumerable<R>> QueryAsync<T, U, V, W, R>(string sql, System.Func<T, U, V, W, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class;

		Task<IEnumerable<T>> QueryAsync<T>(IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<IDataResult<T>> QueryAsync<T>(IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<IEnumerable<T>> QueryAsync<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<IDataResult<T>> QueryAsync<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		T QuerySingle<T>(IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		T QuerySingle<T>(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct;

		T QuerySingle<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		T QuerySingle<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		Task<T> QuerySingleAsync<T>(IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<T> QuerySingleAsync<T>(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct;

		Task<T> QuerySingleAsync<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class;

		Task<T> QuerySingleAsync<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		string QuerySingleString(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		Task<string> QuerySingleStringAsync(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text);

		void SetNullableString(string value, string paramName, DynamicParameters parameters);
	}
}