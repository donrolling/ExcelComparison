using Models.DTO;
using Models.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace Models.Application {
	public class UserContext : IIdentity {
		public long Id { get; set; }
		public string Login { get; set; }
		public string AuthenticationType { get; set; }
		public IEnumerable<MembershipClaim> Claims { get; set; }
		public bool IsAuthenticated { get; set; }
		public string Name { get { return this.Login; } }
	}
}