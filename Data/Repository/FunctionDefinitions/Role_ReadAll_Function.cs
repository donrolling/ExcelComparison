using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class Role_ReadAll_Function : BasePageableFunction
	{
		public Role_ReadAll_Function()
		{
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "Role_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}