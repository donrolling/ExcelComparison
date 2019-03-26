using Dapper;
using Data.Dapper.Interfaces;
using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class Role_SelectById_Function : BaseFunction, ISelectByIdFunction
	{
		public long Id { get; protected set; }
		public override string Signature { get; protected set; } = "@id";

		public Role_SelectById_Function(long id)
		{
			this.Id = id;
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "Role_SelectById";
		}

		public override DynamicParameters DynamicParameters()
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", this.Id);
			return parameters;
		}
	}
}