using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class UserRole_ReadAll_Function : BasePageableFunction
	{
		public UserRole_ReadAll_Function()
		{
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "UserRole_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}