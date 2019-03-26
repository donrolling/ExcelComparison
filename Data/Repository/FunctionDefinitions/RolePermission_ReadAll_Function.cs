using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class RolePermission_ReadAll_Function : BasePageableFunction
	{
		public RolePermission_ReadAll_Function()
		{
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "RolePermission_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}