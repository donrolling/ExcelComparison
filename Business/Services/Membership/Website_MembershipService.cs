using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.Base;
using Common.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Application;
using Models.Base;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Business.Services.Membership
{
	public class Website_MembershipService : EntityServiceBase, IWebsiteMembershipService
	{
		public IOptions<AppSettings> AppSettings { get; set; }
		public IHttpContextAccessor HttpContextAccessor { get; }
		public ISessionCacheService SessionCacheService { get; }
		public IUserService UserService { get; }

		public const string AUTH_SESSION_KEY = "AuthenticationPersistenceService";
		public const string AuthCookie = "AuthCookie";

		private const int _expireMinutes = 20;
		private const string _returnUrlQSParam = "?returnUrl=";
		private static readonly Auditing _auditing = new Auditing();

		public Website_MembershipService(
			IUserService userService,
			ISessionCacheService sessionCacheService,
			IHttpContextAccessor httpContextAccessor,
			IOptions<AppSettings> appSettings,
			ILoggerFactory loggerFactory
		) : base(_auditing, loggerFactory)
		{
			this.UserService = userService;
			this.SessionCacheService = sessionCacheService;
			this.HttpContextAccessor = httpContextAccessor;
			this.AppSettings = appSettings;
		}

		public async Task<UserContext> Current()
		{
			var user = this.HttpContextAccessor.HttpContext.User.Identity;
			if (user == null || string.IsNullOrEmpty(user.Name)) {
				throw new Exception($"Current() - Could not find the current user.");
			}
			var sessionId = getAuthSessionKey(user);
			return await this.SessionCacheService.GetSetAsync<UserContext>(
				sessionId,
				async () =>
				{
					var userContext = await this.hydrate(user.Name, sessionId);
					return userContext;
				},
				true
			);
		}

		public async Task<long> CurrentUserId()
		{
			var user = await this.Current();
			return user.Id;
		}

		public async Task<bool> HasClaim(System.Security.Claims.Claim claim)
		{
			var user = await this.Current();
			if (user.Claims == null) {
				return false;
			}
			//only validate the claim type when the claim value is set to "Any"
			if (claim.Type == NavigationSections.Any.ToString()) {
				return user.Claims.Any(a => a.Type == claim.Type);
			}
			return user.Claims.Any(a => a.Type == claim.Type && a.Value == claim.Value);
		}

		public async Task<bool> SignOut()
		{
			var user = this.HttpContextAccessor.HttpContext.User;
			if (user == null) {
				return true;
			}

			var sessionId = getAuthSessionKey(user.Identity);
			await this.HttpContextAccessor.HttpContext.SignOutAsync();
			await this.SessionCacheService.RemoveAsync(sessionId);
			return true;
		}

		private static string getAuthSessionKey(IIdentity identity)
		{
			return $"{ AUTH_SESSION_KEY }-{ identity.Name }";
		}

		private async Task<UserContext> hydrate(string login, string sessionId)
		{
			var userContext = await this.UserService.GetUserContextByLogin(login);
			if (userContext == null) {
				throw new Exception($"hydrate() - user was not found ({ login }).");
			}

			var identity = new ClaimsIdentity(AuthCookie);
			var userPrincipal = new ClaimsPrincipal(identity);
			var authenticationProperties = new AuthenticationProperties
			{
				ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
				IsPersistent = true,
				AllowRefresh = true
			};
			try {
				await this.HttpContextAccessor.HttpContext.SignInAsync(AuthCookie, userPrincipal, authenticationProperties);
			} catch (Exception ex) {
				this.Logger.LogError(ex, "Sign In Error");
				throw;
			}

			this.SessionCacheService.Remove(sessionId);
			this.SessionCacheService.Set<UserContext>(sessionId, userContext, true);

			return userContext;
		}
	}
}