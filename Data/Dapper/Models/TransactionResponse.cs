using Common.Models;
using Common.Transactions;

using System;
using System.Xml.Serialization;

namespace Data.Dapper.Models {
	public class TransactionResponse : MethodResult {
		/// <summary>
		/// Type of database transaction performed.
		/// </summary>
		public ActionType ActionType { get; set; }
		/// <summary>
		/// Provides the values passed into the parameterized sql string
		/// </summary>
		[XmlIgnore]
		public dynamic Params { get; set; }
		/// <summary>
		/// When possible, the repository will put the sql string here when a transaction fails.
		/// </summary>
		public string SQL { get; set; }
				
		/// <summary>
		/// General enumeration to describe common reasons for transaction failure.
		/// </summary>
		public StatusDetail StatusDetail { get; set; }
		/// <summary>
		/// If a value other than the Id is expected, it can be passed back here.
		/// </summary>
		[XmlIgnore]
		public object TransactionResult { get; set; }

		public static TransactionResponse GetTransactionResponse(ActionType actionType, Status status, StatusDetail statusDetail) {
			return GetTransactionResponse(actionType, status, statusDetail, string.Empty, string.Empty, null);
		}

		public static TransactionResponse GetTransactionResponse(ActionType actionType, Status status, StatusDetail statusDetail, string errorMessage) {
			return GetTransactionResponse(actionType, status, statusDetail, errorMessage, string.Empty, null);
		}

		public static TransactionResponse GetTransactionResponse(ActionType actionType, Status status, StatusDetail statusDetail, string errorMessage, string sql) {
			return GetTransactionResponse(actionType, status, statusDetail, errorMessage, sql, null);
		}

		public static TransactionResponse GetTransactionResponse(ActionType actionType, Status status, StatusDetail statusDetail, string errorMessage, string sql, dynamic parameters) {
			return GetTransactionResponse(actionType, status, statusDetail, errorMessage, sql, parameters, null);
		}

		public static TransactionResponse GetTransactionResponse(ActionType actionType, Status status, StatusDetail statusDetail, string errorMessage, string sql, dynamic parameters, dynamic transactionResult) {
			var result = new TransactionResponse {
				ActionType = actionType,
				Status = status,
				StatusDetail = statusDetail,
				Message = errorMessage,
				SQL = sql,
				Params = parameters,
				Success = status == Status.Success,
				TransactionResult = transactionResult
			};
			return result;
		}

		public void ThrowException() { //todo: you could add more stuff
			throw new Exception(this.Message);
		}
	}
}