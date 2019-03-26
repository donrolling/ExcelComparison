using Dapper;
using Data.Dapper.Interfaces;
using Data.Repository.Dapper.Base;

namespace Data.Repository.FunctionDefinitions
{
	public class RolePermission_SelectById_Function : BaseFunction, ISelectByIdFunction
	{
		public long PermissionId { get; protected set; }
		public long RoleId { get; protected set; }
		public override string Signature { get; protected set; } = "@roleId, @permissionId";

		public RolePermission_SelectById_Function(long roleId, long permissionId)
		{
			this.RoleId = roleId;
			this.PermissionId = permissionId;
			this.DatabaseSchema = "dbo";
			this.UserDefinedFunctionName = "RolePermission_SelectById";
		}

		public override DynamicParameters DynamicParameters()
		{
			var parameters = new DynamicParameters();
			parameters.Add("RoleId", this.RoleId);
			parameters.Add("PermissionId", this.PermissionId);
			return parameters;
		}
	}
}