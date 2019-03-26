using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.Options;
using Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tests.Services
{
	internal class Test_MembershipService : IMembershipService
	{
		public IOptions<AppSettings> AppSettings { get; }
		public IUserService UserService { get; }

		public Test_MembershipService(IOptions<AppSettings> appSettings, IUserService userService) {
			AppSettings = appSettings;
			UserService = userService;
		}

		public async Task<UserContext> Current()
		{
			var login = this.AppSettings.Value.TestLogin;
			return await this.UserService.GetUserContextByLogin(login);
		}

		public async Task<long> CurrentUserId()
		{
			return (await this.Current()).Id;
		}

		public async Task<bool> HasClaim(Claim claim)
		{
			var user = await this.Current();
			return user.Claims.Any(a => a.Type == claim.Type && a.Value == claim.Value);
		}

		public async Task<bool> SignOut()
		{
			return await Task.Run(() => { return true; });
		}
	}
}
