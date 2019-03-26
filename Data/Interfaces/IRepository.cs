using Data.Dapper.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Data.Interfaces {
	public interface IRepository {
		string ConnectionString { get; }
		string DatabaseName { get; }
		string ServerName { get; }

		TransactionResponse Execute(string sql, dynamic param, bool useTransaction = true, int? commandTimeout = null, CommandType? commandType = null);
	}
}