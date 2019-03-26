using Dapper;

namespace Data.Dapper.Interfaces {
	public interface IFunction {
		string DatabaseSchema { get; }
		string Signature { get; }
		string UserDefinedFunctionName { get; }

		DynamicParameters DynamicParameters();
	}
}