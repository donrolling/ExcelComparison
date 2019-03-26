using Dapper;
using Data.Dapper.Interfaces;
using Data.Dapper.Models;

namespace Data.Repository.Dapper.Base {
	public abstract class BasePageableFunction : BaseFunction, IPageableFunction {
		public override string Signature { get; protected set; } = "@readActive, @readInactive";

		public override DynamicParameters DynamicParameters() {
			var parameters = new DynamicParameters();
			parameters.Add("readActive", true);
			parameters.Add("readInactive", false);
			return parameters;
		}

		public virtual DynamicParameters DynamicParameters(PageInfo pageInfo) {
			var parameters = new DynamicParameters();
			parameters.Add("readActive", pageInfo.ReadActive);
			parameters.Add("readInactive", pageInfo.ReadInactive);
			return parameters;
		}
	}
}