using Dapper;
using Data.Dapper.Interfaces;
using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class UserRole_SelectById_Function : BaseFunction, ISelectByIdFunction
	{
		public long RoleId { get; protected set; }
		public override string Signature { get; protected set; } = "@userId, @roleId";
		public long UserId { get; protected set; }

		public UserRole_SelectById_Function(long userId, long roleId)
		{
			this.UserId = userId;
			this.RoleId = roleId;
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "UserRole_SelectById";
		}

		public override DynamicParameters DynamicParameters()
		{
			var parameters = new DynamicParameters();
			parameters.Add("UserId", this.UserId);
			parameters.Add("RoleId", this.RoleId);
			return parameters;
		}
	}
}