using Common.Transactions;
using Microsoft.CSharp.RuntimeBinder;
using Omu.ValueInjecter;
using System;
using System.Runtime.CompilerServices;

namespace Data.Dapper.Models {
	public class InsertResponse<TEntityId> : TransactionResponse where TEntityId : struct {
		/// <summary>
		/// Should be the id of the new or updated entity. This should really only be used in a default scenario.
		/// I don't want to refactor everything, so rather than making InsertResponse<TEntityId>, I've
		/// opted for using the new method: GetTypedResultId()
		/// </summary>
		public TEntityId Id { get; set; }

		/// <summary>
		/// Constructs a new InsertResponse object bearing an Id field of type TEntityId, used when the stored proc output parameter and Id property CLR types do not match.
		/// </summary>
		/// <typeparam name="TSqlId">The CLR type that matches the stored proc's output parameter type</typeparam>
		/// <param name="transactionResponse">The TransactionResponse object returned from the stored procedure call</param>
		/// <param name="keyFieldName">Name of the output parameter representing the primary key field in the entity's database table. If not supplied, defaults to "Id".</param>
		/// <param name="getPropertyValueFunc">A conversion function which returns the TSqlId type value convered into a TEntityId type value</param>
		/// <returns>A new InsertResponse with Id field of type TEntityId.</returns>
		public static InsertResponse<TEntityId> GetInsertResponse<TSqlId>(TransactionResponse transactionResponse, string keyFieldName, Func<TSqlId, TEntityId> getPropertyValueFunc) {
			if (getPropertyValueFunc == null) throw new ArgumentNullException("getPropertyValueFunc must be set to a Func delegate");
			var result = new InsertResponse<TEntityId>();
			result.InjectFrom(transactionResponse);
			result.Id = GetTypedResultId<TSqlId>(transactionResponse.Params, keyFieldName, getPropertyValueFunc);
			return result;
		}

		/// <summary>
		/// Constructs a new InsertResponse object bearing an Id field of type TEntityId, used when the stored proc output parameter and Id property CLR types match
		/// </summary>
		/// <param name="transactionResponse">The TransactionResponse object returned from the stored procedure call</param>
		/// <param name="keyFieldName">Name of the output parameter representing the primary key field in the entity's database table. If not supplied, defaults to "Id".</param>
		/// <returns>A new InsertResponse with Id field of type TEntityId.</returns>
		public static InsertResponse<TEntityId> GetInsertResponse(TransactionResponse transactionResponse, string keyFieldName = "Id") {
			return GetInsertResponse<TEntityId>(transactionResponse, keyFieldName, (v) => v);
		}

		public static InsertResponse<TEntityId> GetInsertResponse(ActionType create, Status failure, StatusDetail statusDetail, TEntityId id) {
			var transactionResponse = TransactionResponse.GetTransactionResponse(create, failure, statusDetail);
			var result = GetInsertResponse(transactionResponse);
			result.Id = id;
			return result;
		}

		public static InsertResponse<TEntityId> GetInsertResponse(ActionType create, Status failure, StatusDetail statusDetail, string validationMessage) {
			var transactionResponse = TransactionResponse.GetTransactionResponse(create, failure, statusDetail, validationMessage);
			return GetInsertResponse(transactionResponse);
		}

		public static object GetProperty(object target, string name) {
			var site = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { CSharpArgumentInfo.Create(0, null) }));
			return site.Target(site, target);
		}

		/// <summary>
		/// Use this method if you have a PK of any type other than long and the stored proc output parameter and Id property CLR types match
		/// </summary>
		/// <typeparam name="TEntityId">The CLR type of the entity's Id property</typeparam>
		/// <param name="parameters">The parameter list passed to the stored procedure call</param>
		/// <param name="keyFieldName">Name of the output parameter representing the primary key field in the entity's database table. If not supplied, defaults to "Id".</param>
		/// <returns>The entity's Id field value based on the value of the primary key output parameter from the stored procedure call</returns>
		public static TEntityId GetTypedResultId(dynamic parameters, string keyFieldName = "Id") {
			return GetTypedResultId<TEntityId>(parameters, keyFieldName, (Func<TEntityId, TEntityId>)((v) => v));
		}

		/// <summary>
		/// Use this method if you have a PK of any type other than long, but the stored proc output parameter and Id property CLR types don't match
		/// </summary>
		/// <typeparam name="TSqlId">The CLR type that matches the stored proc's output parameter type</typeparam>
		/// <typeparam name="TEntityId">The CLR type that matches the entity's Id property</typeparam>
		/// <param name="parameters">The parameter list passed to the stored procedure call</param>
		/// <param name="keyFieldName">Name of the output parameter representing the primary key field in the entity's database table. If not supplied, defaults to "Id".</param>
		/// <param name="getPropertyValueFunc">A conversion function which returns the TSqlId type value convered into a TEntityId type value</param>
		/// <returns>The entity's Id field value based on the value of the primary key output parameter from the stored procedure call</returns>
		public static TEntityId GetTypedResultId<TSqlId>(dynamic parameters, string keyFieldName = "Id", Func<TSqlId, TEntityId> getPropertyValueFunc = null) {
			//todo: need to clean the following code up. The try catch is super wasteful and is happening a lot.
			var result = default(TEntityId);
			try {
				result = getPropertyValueFunc(parameters.Get<TSqlId>(keyFieldName));
			} catch {
				try {
					var prop = GetProperty(parameters, keyFieldName);
					if (prop == null) {
						return result;
					}
					return (TEntityId)getPropertyValueFunc(prop);
				} catch { }
			}
			return result;
		}
	}
}