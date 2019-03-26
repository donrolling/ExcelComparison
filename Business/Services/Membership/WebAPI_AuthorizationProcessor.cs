using Business.Interfaces;
using Business.Models.Membership;
using Models.Application;
using System;

namespace Business.Services.Membership
{
	public class WebAPI_AuthorizationProcessor : IAuthorizationProcessor
	{
		public string LoginUrl { get; private set; }
		public string UnauthorizedUrl { get; private set; }

		public WebAPI_AuthorizationProcessor()
		{
		}

		public AuthorizationResult Authorize(string url, UserContext user, RoleType roleType)
		{
			//otherwise, just figure out if he should see this page or not
			//attempting to identify the businessUnit by url in order to authorize the user
			var hasPermission = this.HasPermission(user, 0, roleType);
			if (hasPermission) {
				return new AuthorizationResult(AuthorizationResultDetail.Success);
			} else {
				return new AuthorizationResult(AuthorizationResultDetail.AccessDenied, this.UnauthorizedUrl);
			}
		}

		public bool HasPermission(UserContext user, long businessUnitId, RoleType roleType)
		{
			throw new NotImplementedException();
		}

		public AuthorizationResult Unauthorized()
		{
			throw new NotImplementedException();
		}
	}
}