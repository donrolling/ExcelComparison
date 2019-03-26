using Data.Dapper.Interfaces;
using Data.Dapper.Models;
using Models.Base;
using Data.Repository.Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Interfaces;

namespace Data.Interfaces {
	public interface IEntityDapperRepository<T, U> : IDapperAsyncRepository where T : BaseEntity<U> where U : struct {

		Task<InsertResponse<U>> Create(T entity);

		Task<TransactionResponse> Delete(U id);

		Task<IDataResult<T>> ReadAll(PageInfo pageInfo);

		Task<T> SelectById(U id);

		Task<TransactionResponse> Update(T entity);
	}
}