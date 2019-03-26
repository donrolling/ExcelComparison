using Data.Dapper.Interfaces;
using Data.Dapper.Models;
using Models.Base;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Data.Repository.Interfaces
{
	public interface IAssociativeDapperRepository<T, U, V> : IDapperAsyncRepository where T : Association<U, V> where U : struct
	{
		Task<TransactionResponse> Create(T entity);

		Task<TransactionResponse> Delete(U id1, V id2);

		Task<IDataResult<T>> ReadAll(PageInfo pageInfo);

		Task<T> SelectById(U id1, V id2);

		Task<TransactionResponse> Update(T entity);
	}
}