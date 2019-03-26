using Dapper;
using Data.Dapper.Interfaces;

namespace Data.Repository.Dapper.Base {
	public abstract class BaseFunction : IFunction {
		public string DatabaseSchema { get; protected set; }
		public virtual string Signature { get; protected set; }
		public string UserDefinedFunctionName { get; protected set; }

		public abstract DynamicParameters DynamicParameters();
	}
}