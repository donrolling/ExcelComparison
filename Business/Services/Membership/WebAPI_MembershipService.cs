using Business.Interfaces;
using Business.Security;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Web.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.Membership
{
	public class WebAPI_MembershipService : EntityServiceBase, IWebAPIMembershipService
	{
		public IOptions<AppSettings> AppSettings { get; set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public IJWTService JwtService { get; }
		public ISessionCacheService SessionCacheService { get; }
		public IUserService UserService { get; }

		private const string _returnUrlQSParam = "?returnUrl=";
		private const string JWT_HeaderName = "Authorization";
		private static readonly Auditing _auditing = new Auditing();
		private UserContext _userContext;

		public WebAPI_MembershipService(
			IJWTService jwtService,
			IUserService userService,
			ISessionCacheService sessionCacheService,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings,
			ILoggerFactory loggerFactory
		) : base(_auditing, loggerFactory)
		{
			this.JwtService = jwtService;
			this.UserService = userService;
			this.SessionCacheService = sessionCacheService;
			this.HttpContextAccessor = httpContextAccessor;
			this.AppSettings = appSettings;
		}

		public async Task<UserContext> Current()
		{
			if (this._userContext != null) {
				//this.Logger.LogInformation($"WebAPI_MembershipService Current(): { this._userContext.Name } was already loaded.");
				return this._userContext;
			}
			var authorizationHeaderExists = this.HttpContextAccessor.HttpContext.Request.Headers.Any(a => a.Key == JWT_HeaderName);
			if (!authorizationHeaderExists) {
				throw new Exception($"Authorization Header ({ JWT_HeaderName }) was missing.");
			}
			var authorizationHeader = this.HttpContextAccessor.HttpContext.Request.Headers
													.First(a => a.Key == JWT_HeaderName);
			var jwt = authorizationHeader.Value.First();
			if (string.IsNullOrEmpty(jwt)) {
				throw new Exception($"Could not properly read the authorization header ({ JWT_HeaderName }).");
			}
			var verifyTokenEnv = this.JwtService.VerifyToken(jwt);
			if (verifyTokenEnv.Failure) {
				throw new Exception($"Could not properly validate the JWT in the authorization header ({ JWT_HeaderName }).\r\n\t{ verifyTokenEnv.Message }");
			}
			var token = verifyTokenEnv.Result;
			var hasUserKey = token.Any(a => a.Key == TokenKeys.Login);
			if (!hasUserKey) {
				throw new Exception($"Could not properly validate the JWT in the authorization header ({ JWT_HeaderName }).\r\n\tLogin key not found.\r\n\t{ verifyTokenEnv.Message }");
			}
			var user = token.First(a => a.Key == TokenKeys.Login);
			var userName = user.Value.ToString();
			if (string.IsNullOrEmpty(userName)) {
				throw new Exception($"Could not properly validate the JWT in the authorization header ({ JWT_HeaderName }).\r\n\tLogin value was empty.\r\n\t{ verifyTokenEnv.Message }");
			}
			this._userContext = await this.UserService.GetUserContextByLogin(userName);
			if (this._userContext == null) {
				throw new Exception($"The login that was provided in the authorization header ({ JWT_HeaderName } : { userName }) did not match any login in the database.");
			}
			return this._userContext;
		}

		public async Task<long> CurrentUserId()
		{
			var user = await this.Current();
			return user.Id;
		}

		public async Task<bool> HasClaim(System.Security.Claims.Claim claim)
		{
			var user = await this.Current();
			//only validate the claim type when the claim value is set to "Any"
			if (claim.Type == NavigationSections.Any.ToString()) {
				return user.Claims.Any(a => a.Type == claim.Type);
			}
			return user.Claims.Any(a => a.Type == claim.Type && a.Value == claim.Value);
		}

		public async Task<bool> SignOut()
		{
			await Task.Run(() => _userContext = null);
			return true;
		}
	}
}