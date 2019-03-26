using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class User_ReadAll_Function : BasePageableFunction
	{
		public User_ReadAll_Function()
		{
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "User_ReadAll";
			this.Signature = "@readActive, @readInactive";
		}
	}
}