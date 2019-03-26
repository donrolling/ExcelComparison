using Business.Interfaces;
using Business.Security;
using Business.Security.WebAPI;
using Business.Services.Membership;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Application;
using Omu.ValueInjecter;
using System.Threading.Tasks;
using Website.Models;

namespace Website.Controllers
{
	[ServiceFilter(typeof(Website_SecurityFilter))]
	public class SettingsController : Controller
	{
		public IOptions<AppSettings> AppSettings { get; }
		public IMembershipService MembershipService { get; }

		public SettingsController(
			IMembershipService membershipService,
			IOptions<AppSettings> appSettings
		)
		{
			MembershipService = membershipService;
			AppSettings = appSettings;
		}


		public async Task<ClientAppSettings> GetAppSettings()
		{
			var clientAppSettings = new ClientAppSettings();
			clientAppSettings.InjectFrom(this.AppSettings.Value);

			var user = await this.MembershipService.Current();
			if (user == null) {
				return clientAppSettings;
			}

			clientAppSettings.Login = user.Login;
			return clientAppSettings;
		}

		public async Task<bool> SignOut()
		{
			await this.MembershipService.SignOut();
			return true;
		}
	}
}