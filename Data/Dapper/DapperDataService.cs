using Common.Extensions;
using Common.Logging;
using Dapper;
using Data.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common.Interfaces;
using Microsoft.Extensions.Logging;
using Common.BaseClasses;
using Data.Dapper.Models;
using Data.Dapper.Enums;
using Data.Interfaces;
using Models;
using Common.Transactions;
using Models.Interfaces;

namespace Data.Repository.Dapper.Base {
	public class DapperDataService : LoggingWorker {

		public DapperDataService(ILoggerFactory loggerFactory) : base(loggerFactory) {
		}

		public TransactionResponse execute(string connectionString, string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null) {
			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				var result = 0;
				using (var connection = new SqlConnection(connectionString)) {
					connection.Open();
					if (useTransaction) {
						using (var transaction = connection.BeginTransaction()) {
							result = SqlMapper.Execute(connection, sql, param, transaction, commandTimeout, commandType);
							transaction.Commit();
						}
					} else {
						result = SqlMapper.Execute(connection, sql, param, null, commandTimeout, commandType);
					}
				}
				return TransactionResponse.GetTransactionResponse(ActionType.Execute, status, StatusDetail.OK, errorMessage, sql, param, result);
			} catch (Exception ex) {
				status = Status.Failure;
				Logger.LogError(ex, $"ActionType: { ActionType.Execute }\r\nsql: { sql }\r\nparams: { param }");
				throw;
			}
		}

		public IEnumerable<object> query(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = SqlMapper.Query(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					throw;
				}
			}
		}

		public IEnumerable<T> query<T>(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = SqlMapper.Query<T>(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//return new List<T>();
					throw;
				}
			}
		}

		public IEnumerable<R> query<T, U, R>(string connectionString, string sql, Func<T, U, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var result = SqlMapper.Query<T, U, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//return new List<R>();
					throw;
				}
			}
		}

		public IEnumerable<T> query<T>(string connectionString, IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, functionCall.Signature, functionCall.DatabaseSchema, functionCall.UserDefinedFunctionName);
			return this.query<T>(connectionString, sql, functionCall.DynamicParameters(), commandTimeout);
		}

		public IEnumerable<T> query<T>(string connectionString, string functionName, string signature, DynamicParameters dynamicParameters, string databaseSchema, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(string.Empty, signature, databaseSchema, functionName);
			return this.query<T>(connectionString, sql, dynamicParameters, commandTimeout);
		}

		public IEnumerable<R> query<T, U, V, R>(string connectionString, string sql, Func<T, U, V, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var result = SqlMapper.Query<T, U, V, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout);
					return result;
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//return new List<R>();
					throw;
				}
			}
		}

		public IEnumerable<R> query<T, U, V, W, R>(string connectionString, string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = SqlMapper.Query<T, U, V, W, R>(connection, sql, func, param, splitOn: splitOn, commandTimeout: commandTimeout, commandType: _commandType);
					return result;
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//return new List<R>();
					throw;
				}
			}
		}

		public IDataResult<T> query<T>(string connectionString, string functionName, string signature, DynamicParameters dynamicParameters, PageInfo pageInfo, string databaseSchema = "[dbo]", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var selectedFields = pageInfo.SelectedColumns.Any()
				? pageInfo.SelectedColumns.ToCsv()
				: "*";
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, signature, databaseSchema, functionName);
			return this.query<T>(connectionString, sql, dynamicParameters, pageInfo, commandTimeout);
		}

		public IDataResult<T> query<T>(string connectionString, IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var dynamicParameters = pageableFunctionCall.DynamicParameters(pageInfo);
			var selectedFields = pageInfo.SelectedColumns.Any()
				? pageInfo.SelectedColumns.ToCsv()
				: "*";
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, pageableFunctionCall.Signature, pageableFunctionCall.DatabaseSchema, pageableFunctionCall.UserDefinedFunctionName);
			return this.query<T>(connectionString, sql, dynamicParameters, pageInfo, commandTimeout);
		}

		public IDataResult<T> query<T>(string connectionString, string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var prep = SQL_Helper.PrepForPresentationListResult(sql, dynamicParameters, pageInfo);
			var data = this.query<T>(connectionString, prep.Query, dynamicParameters, commandTimeout);
			var total = this.querySingle<int>(connectionString, prep.TotalQuery, dynamicParameters, commandTimeout);
			var result = new DataResult<T>(data, pageInfo.PageSize, total);
			return result;
		}

		public T querySingle<T>(string connectionString, ISelectByIdFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return this.querySingle<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public T querySingle<T>(string connectionString, IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class {
			var sql = SQL_Helper.GetTableValuedFunctionCallSQL(selectedFields, function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return this.querySingle<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public T querySingle<T>(string connectionString, IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct {
			var sql = SQL_Helper.GetScalarFunctionCallSQL(function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return querySingle<T>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}

		public T querySingle<T>(string connectionString, string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			using (var connection = new SqlConnection(connectionString)) {
				connection.Open();
				try {
					var _commandType = commandType.HasValue ? commandType.Value : CommandType.Text;
					var result = SqlMapper.Query<T>(connection, sql, param, commandType: _commandType, commandTimeout: commandTimeout);
					if (result.Count == 0) {
						return default(T);
					}
					return result[0];
				} catch (Exception ex) {
					Logger.LogError(ex, sql);
					//todo: should we actually return a new list here or throw?
					//T retval = default(T);
					//return retval;
					throw;
				}
			}
		}

		public string querySingleString(string connectionString, IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) {
			var sql = SQL_Helper.GetScalarFunctionCallSQL(function.Signature, function.DatabaseSchema, function.UserDefinedFunctionName);
			return querySingle<string>(connectionString, sql, function.DynamicParameters(), commandTimeout);
		}
	}
}