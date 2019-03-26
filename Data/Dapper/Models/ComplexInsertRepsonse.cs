using System.Collections.Generic;

namespace Data.Dapper.Models
{
    public class ComplexInsertResponse<TEntityId> : InsertResponse<TEntityId> where TEntityId : struct
	{
		public List<TransactionResponse> TransactionResponses { get; set; }
	}
}