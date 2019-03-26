using Dapper;

namespace Data.Dapper.Interfaces {
	public interface IScalarFunction {
		string DatabaseSchema { get; }
		string Signature { get; }
		string UserDefinedFunctionName { get; }

		DynamicParameters DynamicParameters();
	}
}