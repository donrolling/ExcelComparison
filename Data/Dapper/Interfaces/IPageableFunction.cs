using Dapper;
using Data.Dapper.Models;

namespace Data.Dapper.Interfaces {
	public interface IPageableFunction : IFunction {

		DynamicParameters DynamicParameters(PageInfo pageInfo);
	}
}