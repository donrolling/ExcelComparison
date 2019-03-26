using Business.Security;
using Business.Security.WebAPI;
using Business.Service.EntityServices.Interfaces;
using Business.Services.Membership;
using Common.Web.BaseClasses;
using Data.Dapper.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Website.Controllers
{
	[ServiceFilter(typeof(Website_SecurityFilter))]
	[ClaimRequirement(NavigationSections.Permission, ClaimActions.View)]
	public class PermissionController : LoggingController
	{
		public IPermissionService PermissionService { get; }

		public PermissionController(IPermissionService permissionService, ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment) : base(loggerFactory, hostingEnvironment)
		{
			PermissionService = permissionService;
		}

		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Create)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Create)]
		public async Task<IActionResult> Create(Permission permission)
		{
			var result = await PermissionService.Create(permission);
			return RedirectToAction("Index");
		}

		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(long? id = null)
		{
			if (id.HasValue) {
				var permission = await PermissionService.SelectById(id.Value);
				if (permission != null) {
					return View(permission);
				}
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(Permission permission)
		{
			var result = await PermissionService.Delete(permission);
			return RedirectToAction("Index");
		}

		[ClaimRequirement(NavigationSections.Permission, ClaimActions.View)]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Permission, ClaimActions.View)]
		public async Task<IDataResult<Permission>> ReadAll(ClientPageInfo clientPageInfo)
		{
			return await PermissionService.ReadAll(clientPageInfo);
		}

		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Edit)]
		public async Task<IActionResult> Update(long? id = null)
		{
			if (id.HasValue) {
				var permission = await PermissionService.SelectById(id.Value);
				if (permission != null) {
					return View(permission);
				}
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Permission, ClaimActions.Edit)]
		public async Task<IActionResult> Update(Permission permission)
		{
			var result = await PermissionService.Update(permission);
			return RedirectToAction("Index");
		}
	}
}