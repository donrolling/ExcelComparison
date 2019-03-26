using Dapper;
using Data.Repository.Dapper.Base;
using Data.Dapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common.Interfaces;
using Common.BaseClasses;
using Microsoft.Extensions.Logging;
using Data.Dapper.Models;
using Data.Interfaces;
using Models.Interfaces;

namespace Data.Repository.Dapper {
	public class DapperRepository : LoggingWorker, IDapperRepository {
		public string ConnectionString {
			get {
				if (_connectionString == null) {
					throw new Exception("Repository must be initialized with a connection string.");
				}
				return _connectionString;
			}
			private set {
				_connectionString = value;
				var builder = new SqlConnectionStringBuilder(_connectionString);
				ServerName = builder.DataSource;
				DatabaseName = builder.InitialCatalog;
			}
		}
		public DapperDataService DapperDataService { get; private set; }
		public string DatabaseName { get; private set; }
		public string ServerName { get; private set; }
		private string _connectionString;

		public DapperRepository(string connectionString, ILoggerFactory loggerFactory) : base(loggerFactory) {
			this.ConnectionString = connectionString;
			this.DapperDataService = new DapperDataService(loggerFactory);
		}

		public TransactionResponse Execute(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null) {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.execute(connectionString, sql, new { }, useTransaction, commandTimeout, commandType);
		}

		public TransactionResponse Execute(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null) {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.execute(connectionString, sql, param, useTransaction, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(string sql, dynamic param) where T : class {
			return this.DapperDataService.query<T>(this.ConnectionString, sql, param);
		}

		public IEnumerable<T> Query<T>(IFunction functionCall, string selectedFields = "") where T : class {
			return this.DapperDataService.query<T>(this.ConnectionString, functionCall, selectedFields);
		}

		public IEnumerable<R> Query<T, U, R>(string sql, Func<T, U, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where R : class {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, R>(connectionString, sql, func, param, splitOn);
		}

		public IEnumerable<R> Query<T, U, V, R>(string sql, Func<T, U, V, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where R : class
			where V : class {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, V, R>(connectionString, sql, func, param, splitOn);
		}

		public IEnumerable<R> Query<T, U, V, W, R>(string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, V, W, R>(connectionString, sql, func, param, splitOn);
		}

		public IDataResult<T> Query<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo) where T : class {
			return this.DapperDataService.query<T>(this.ConnectionString, sql, dynamicParameters, pageInfo);
		}

		public IDataResult<T> Query<T>(IPageableFunction pageableFunctionCall, PageInfo pageInfo) where T : class {
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T>(connectionString, pageableFunctionCall, pageInfo);
		}

		public T QuerySingle<T>(string sql, dynamic param) {
			return this.DapperDataService.querySingle<T>(this.ConnectionString, sql, param);
		}

		public T QuerySingle<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "") where T : class {
			return this.DapperDataService.querySingle<T>(this.ConnectionString, selectByIdFunction, selectedFields);
		}

		public T QuerySingle<T>(IFunction function, string selectedFields = "") where T : class {
			return this.DapperDataService.querySingle<T>(this.ConnectionString, function, selectedFields);
		}

		public T QuerySingle<T>(IScalarFunction function) where T : struct {
			return this.DapperDataService.querySingle<T>(this.ConnectionString, function);
		}

		public string QuerySingleString(IScalarFunction function) {
			return this.DapperDataService.querySingleString(this.ConnectionString, function);
		}
	}
}