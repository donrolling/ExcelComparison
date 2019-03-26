using Dapper;
using Data.Dapper.Interfaces;

namespace Data.Repository.Dapper.Base {
	public abstract class BaseSelectByIdFunction : BaseFunction, ISelectByIdFunction {
		public long Id { get; protected set; }
		public override string Signature { get; protected set; } = "@id";

		public override DynamicParameters DynamicParameters() {
			var parameters = new DynamicParameters();
			parameters.Add("Id", this.Id);
			return parameters;
		}
	}
}