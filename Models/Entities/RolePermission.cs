using Models.Base;

namespace Models.Entities
{
	public class RolePermission : Association<long, long>
	{
		public long PermissionId {
			get {
				return this.FK_Two;
			}
			set {
				this.FK_Two = value;
			}
		}

		public long RoleId {
			get {
				return this.FK_One;
			}
			set {
				this.FK_One = value;
			}
		}
	}

	public class RolePermission_Properties : Entity_Properties
	{
		public const string PermissionId = "PermissionId";
		public const string RoleId = "RoleId";
	}
}