using Data.Dapper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces {
	public interface IAsyncRepository {
		string ConnectionString { get; }
		string DatabaseName { get; }
		string ServerName { get; }

		Task<TransactionResponse> ExecuteAsync(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);
	}
}