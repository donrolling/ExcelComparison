using Models.Base;

namespace Models.Entities
{
	public class UserRole : Association<long, long>
	{
		public long RoleId {
			get {
				return this.FK_One;
			}
			set {
				this.FK_One = value;
			}
		}

		public long UserId {
			get {
				return this.FK_Two;
			}
			set {
				this.FK_Two = value;
			}
		}
	}

	public class UserRole_Properties : Entity_Properties
	{
		public const string RoleId = "RoleId";
		public const string UserId = "UserId";
	}
}