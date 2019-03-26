using Common.BaseClasses;
using Dapper;
using Data.Dapper.Interfaces;
using Data.Dapper.Models;
using Data.Repository.Dapper.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Data.Repository.Dapper
{
	public class DapperAsyncRepository : LoggingWorker, IDapperAsyncRepository
	{
		public IOptions<AppSettings> AppSettings { get; private set; }
		public string ConnectionString {
			get {
				if (_connectionString == null)
				{
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
		public DapperAsyncDataService DapperAsyncDataService { get; private set; }
		public DapperDataService DapperDataService { get; private set; }
		public string DatabaseName { get; private set; }
		public string ServerName { get; private set; }
		private string _connectionString = string.Empty;

		public DapperAsyncRepository(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			initialize(connectionString, appSettings, loggerFactory);
		}

		public DapperAsyncRepository(IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			initialize(string.Empty, appSettings, loggerFactory);
		}

		public TransactionResponse Execute(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.execute(connectionString, sql, new { }, useTransaction, commandTimeout, commandType);
		}

		//---------------------------------------------------------------------------
		//Async Methods
		//---------------------------------------------------------------------------
		//---------------------------------------------------------------------------
		//Standard Methods
		//---------------------------------------------------------------------------
		public TransactionResponse Execute(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.execute(connectionString, sql, param, useTransaction, commandTimeout, commandType);
		}

		public async Task<TransactionResponse> ExecuteAsync(string sql, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.ExecuteAsync(connectionString, sql, new { }, useTransaction, commandTimeout, commandType);
		}

		public async Task<TransactionResponse> ExecuteAsync(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.ExecuteAsync(connectionString, sql, param, useTransaction, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return this.DapperDataService.query<T>(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public IEnumerable<T> Query<T>(IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return this.DapperDataService.query<T>(this.ConnectionString, functionCall, selectedFields, commandTimeout, commandType);
		}

		public IEnumerable<R> Query<T, U, R>(string sql, Func<T, U, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public IEnumerable<R> Query<T, U, V, R>(string sql, Func<T, U, V, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class
			where V : class
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, V, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public IEnumerable<R> Query<T, U, V, W, R>(string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T, U, V, W, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public IDataResult<T> Query<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return this.DapperDataService.query<T>(this.ConnectionString, sql, dynamicParameters, pageInfo, commandTimeout, commandType);
		}

		public IDataResult<T> Query<T>(IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			var connectionString = this.ConnectionString;
			return this.DapperDataService.query<T>(connectionString, pageableFunctionCall, pageInfo, commandTimeout, commandType);
		}

		public IEnumerable<object> Query(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return this.DapperDataService.query(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string sql, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsync<T>(this.ConnectionString, sql, null, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsync<T>(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(IFunction functionCall, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsync<T>(this.ConnectionString, functionCall, selectedFields, commandTimeout, commandType);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, R>(string sql, Func<T, U, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.QueryAsync<T, U, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, V, R>(string sql, Func<T, U, V, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where R : class
			where V : class
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.QueryAsync<T, U, V, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public async Task<IEnumerable<R>> QueryAsync<T, U, V, W, R>(string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.QueryAsync<T, U, V, W, R>(connectionString, sql, func, param, splitOn, commandTimeout, commandType);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(string sql, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsyncFinal<T>(this.ConnectionString, sql, new DynamicParameters(), pageInfo, commandTimeout, commandType);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(string sql, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsyncFinal<T>(this.ConnectionString, sql, dynamicParameters, pageInfo, commandTimeout, commandType);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(IPageableFunction pageableFunctionCall, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.QueryAsync<T>(connectionString, pageableFunctionCall, pageInfo, commandTimeout, commandType);
		}

		public async Task<IDataResult<T>> QueryAsync<T>(IPageableFunction pageableFunctionCall, DynamicParameters dynamicParameters, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			var connectionString = this.ConnectionString;
			return await this.DapperAsyncDataService.QueryAsync<T>(connectionString, pageableFunctionCall, dynamicParameters, pageInfo, commandTimeout, commandType);
		}

		public async Task<IEnumerable<T>> QueryAsync<T>(string sql, dynamic param, PageInfo pageInfo, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QueryAsyncFinal<T>(this.ConnectionString, sql, param, pageInfo, commandTimeout, commandType);
		}

		public async Task<IEnumerable<object>> QueryAsync(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return await this.DapperAsyncDataService.QueryAsync(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public T QuerySingle<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return this.DapperDataService.querySingle<T>(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public T QuerySingle<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return this.DapperDataService.querySingle<T>(this.ConnectionString, selectByIdFunction, selectedFields, commandTimeout, commandType);
		}

		public T QuerySingle<T>(IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return this.DapperDataService.querySingle<T>(this.ConnectionString, function, selectedFields, commandTimeout, commandType);
		}

		public T QuerySingle<T>(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct
		{
			return this.DapperDataService.querySingle<T>(this.ConnectionString, function, commandTimeout, commandType);
		}

		public async Task<T> QuerySingleAsync<T>(string sql, dynamic param, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return await this.DapperAsyncDataService.QuerySingleAsync<T>(this.ConnectionString, sql, param, commandTimeout, commandType);
		}

		public async Task<T> QuerySingleAsync<T>(ISelectByIdFunction selectByIdFunction, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QuerySingleAsync<T>(this.ConnectionString, selectByIdFunction, selectedFields, commandTimeout, commandType);
		}

		public async Task<T> QuerySingleAsync<T>(IFunction function, string selectedFields = "", int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : class
		{
			return await this.DapperAsyncDataService.QuerySingleAsync<T>(this.ConnectionString, function, selectedFields, commandTimeout, commandType);
		}

		public async Task<T> QuerySingleAsync<T>(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text) where T : struct
		{
			return await this.DapperAsyncDataService.QuerySingleAsync<T>(this.ConnectionString, function, commandTimeout, commandType);
		}

		public string QuerySingleString(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return this.DapperDataService.querySingleString(this.ConnectionString, function, commandTimeout, commandType);
		}

		public async Task<string> QuerySingleStringAsync(IScalarFunction function, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
		{
			return await this.DapperAsyncDataService.QuerySingleStringAsync(this.ConnectionString, function, commandTimeout, commandType);
		}

		public void SetNullableString(string value, string paramName, DynamicParameters parameters)
		{
			if (string.IsNullOrEmpty(value))
			{
				parameters.Add(paramName, null);
			}
			else
			{
				parameters.Add(paramName, value);
			}
		}

		private void initialize(string connectionString, IOptions<AppSettings> appSettings, ILoggerFactory loggerFactory)
		{
			this.AppSettings = appSettings;
			//allowing a default behaviour, but allowing for overloading that behaviour
			_connectionString = connectionString;
			if (string.IsNullOrEmpty(_connectionString))
			{
				this.ConnectionString = appSettings.Value.ConnectionStrings.DefaultConnection;
			}
			this.DapperAsyncDataService = new DapperAsyncDataService(loggerFactory);
			this.DapperDataService = new DapperDataService(loggerFactory);
		}
	}
}