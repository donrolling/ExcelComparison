using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class Permission_ReadAll_Function : BasePageableFunction
	{
		public Permission_ReadAll_Function()
		{
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "Permission_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}