using Dapper;
using Data.Dapper.Models;
using Data.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Data.Dapper.Interfaces {
	public interface IDapperRepository : IRepository {
		ILogger Logger { get; }

		TransactionResponse Execute(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);

		IEnumerable<T> Query<T>(string sql, dynamic param) where T : class;

		IDataResult<T> Query<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo) where T : class;

		IEnumerable<T> Query<T>(IFunction function, string selectedFields = "") where T : class;

		IDataResult<T> Query<T>(IPageableFunction pageablefunction, PageInfo pageInfo) where T : class;

		IEnumerable<R> Query<T, U, R>(string sql, System.Func<T, U, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where R : class;

		IEnumerable<R> Query<T, U, V, R>(string sql, System.Func<T, U, V, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where V : class
			where R : class;

		IEnumerable<R> Query<T, U, V, W, R>(string sql, System.Func<T, U, V, W, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class;

		T QuerySingle<T>(string sql, dynamic param);

		T QuerySingle<T>(IScalarFunction function) where T : struct;

		T QuerySingle<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "") where T : class;

		T QuerySingle<T>(IFunction function, string selectedFields = "") where T : class;

		string QuerySingleString(IScalarFunction function);
	}
}