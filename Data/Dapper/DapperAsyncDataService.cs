using Common.BaseClasses;
using Common.Extensions;
using Common.Transactions;
using Dapper;
using Data.Dapper.Interfaces;
using Data.Dapper.Models;
using Microsoft.Extensions.Logging;
using Models;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repository.Dapper.Base {
	public class DapperAsyncDataService : LoggingWorker {

		public DapperAsyncDataService(ILoggerFactory loggerFactory) : base(loggerFactory) {
		}

		public async Task<TransactionResponse> ExecuteAsync(string connectionString, string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null) {
			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				var result = 0;
				using (var connection = new SqlConnection(connectionString)) {
					await connection.OpenAsync();
					if (useTransaction) {
						using (var transaction = connection.BeginTransaction()) {
							result = await SqlMapper.ExecuteAsync(connection, sql, param, transaction, commandTimeout, commandType);
							transaction.Commit();
						}
					} else {
						result = await SqlMapper.ExecuteAsync(connection, sql, param);
					}
				}
				return TransactionResponse.GetTransactionResponse(ActionType.Execute, status, StatusDetail.OK, errorMessage, sql, param, result);
			} catch (Exception ex) {
				status = Status.Failure;
				this.Logger.LogError(ex, $"ActionType: { ActionType.Execute }\r\nsql: { sql }\r\nparams: { param }");
				throw;
			}
		}

		public async Task<IEnumerable<object>> QueryAsync(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			return await this.queryAsync(connectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			return await this.queryAsync<T>(connectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, R>(string connectionString, string sql, Func<T, U, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class {
			return await this.queryAsync<T, U, R>(connectionString, sql, func, param, splitOn, commandTimeout);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, functionCall.Signature, functionCall.DatabaseSchema, functionCall.UserDefinedFunctionName);
			var param = functionCall.DynamicParameters();
			return await this.QueryAsync<T>(connectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string connectionString, string functionName, string signature, DynamicParameters dynamicParameters, string databaseSchema, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(string.Empty, signature, databaseSchema, functionName);
			return await this.QueryAsync<T>(connectionString, sql, dynamicParameters, commandTimeout);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, V, R>(string connectionString, string sql, Func<T, U, V, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where R : class {
			return await this.queryAsync<T, U, V, R>(connectionString, sql, func, param, splitOn, commandTimeout);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, V, W, R>(string connectionString, string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class {
			return await this.queryAsync<T, U, V, W, R>(connectionString, sql, func, param, splitOn, commandTimeout);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(string connectionString, string functionName, string signature, DynamicParameters dynamicParameters, PageInfo pageInfo, string databaseSchema = "[dbo]", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var selectedFields = pageInfo.SelectedColumns.Any()
				? pageInfo.SelectedColumns.ToCsv()
				: "*";
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, signature, databaseSchema, functionName);
			return await this.QueryAsyncFinal<T>(connectionString, sql, dynamicParameters, pageInfo, commandTimeout);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(string connectionString, IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var dynamicParameters = pageableFunctionCall.DynamicParameters(pageInfo);
			var selectedFields = pageInfo.SelectedColumns.Any()
				? pageInfo.SelectedColumns.ToCsv()
				: "*";
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, pageableFunctionCall.Signature, pageableFunctionCall.DatabaseSchema, pageableFunctionCall.UserDefinedFunctionName);
			return await this.QueryAsyncFinal<T>(connectionString, sql, dynamicParameters, pageInfo, commandTimeout);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(string connectionString, IPageableFunction pageableFunctionCall, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			dynamicParameters.AddDynamicParams(pageableFunctionCall.DynamicParameters(pageInfo));
			var selectedFields = pageInfo.SelectedColumns.Any()
				? pageInfo.SelectedColumns.ToCsv()
				: "*";
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, pageableFunctionCall.Signature, pageableFunctionCall.DatabaseSchema, pageableFunctionCall.UserDefinedFunctionName);
			return await this.QueryAsyncFinal<T>(connectionString, sql, dynamicParameters, pageInfo, commandTimeout);
		}

		public async Task<IDataResult<T>> QueryAsyncFinal<T>(string connectionString, string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var prep = SQL_Helper.PrepForPresentationListResult(sql, dynamicParameters, pageInfo);
			var data = await this.QueryAsync<T>(connectionString, prep.Query, dynamicParameters, commandTimeout);
			var total = await this.QuerySingleAsync<int>(connectionString, prep.TotalQuery, dynamicParameters, commandTimeout);
			var result = new DataResult<T>(data, pageInfo.PageSize, total);
			return result;
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, ISelectByIdFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return await this.QuerySingleAsync<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return await this.QuerySingleAsync<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct {
			var sql = SQL_Helper.GetScalarFunctionCallSQL(function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return await QuerySingleAsync<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public async Task<T> QuerySingleAsync<T>(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync<T>(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					if (result.Count == 0) {
						return default(T);
					}
					return result[0];
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//T retval = default(T);
					//return retval;
					throw;
				}
			}
		}

		public async Task<string> QuerySingleStringAsync(string connectionString, IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			var sql = SQL_Helper.GetScalarFunctionCallSQL(function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return await QuerySingleAsync<string>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public async Task<string> QuerySingleStringAsync(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			return await QuerySingleAsync<string>(connectionString, sql, param, commandTimeout, commandType);
		}

		private async Task<IEnumerable<object>> queryAsync(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					throw;
				}
			}
		}

		private async Task<IEnumerable<T>> queryAsync<T>(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync<T>(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					throw;
				}
			}
		}

		private async Task<IEnumerable<R>> queryAsync<T, U, R>(string connectionString, string sql, Func<T, U, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync<T, U, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout, commandType: _commandType);
					return result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					throw;
				}
			}
		}

		private async Task<IEnumerable<R>> queryAsync<T, U, V, R>(string connectionString, string sql, Func<T, U, V, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync<T, U, V, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout, commandType: _commandType);
					return result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					throw;
				}
			}
		}

		private async Task<IEnumerable<R>> queryAsync<T, U, V, W, R>(string connectionString, string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				try {
					await connection.OpenAsync();
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = await SqlMapper.QueryAsync<T, U, V, W, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout, commandType: _commandType);
					return result;
				} catch (Exception ex) {
					this.Logger.LogError(ex, sql);
					throw;
				}
			}
		}
	}
}