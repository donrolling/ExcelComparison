using Business.Security;
using Business.Security.WebAPI;
using Business.Service.EntityServices.Interfaces;
using Business.Services.Membership;
using Common.Web.BaseClasses;
using Data.Dapper.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Website.Controllers
{
	[ServiceFilter(typeof(Website_SecurityFilter))]
	[ClaimRequirement(NavigationSections.Role, ClaimActions.View)]
	public class RoleController : LoggingController
	{
		public IPermissionService PermissionService { get; }
		public IRoleService RoleService { get; }

		public RoleController(IRoleService roleService, IPermissionService permissionService, ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment) : base(loggerFactory, hostingEnvironment)
		{
			RoleService = roleService;
			PermissionService = permissionService;
		}

		[ClaimRequirement(NavigationSections.Role, ClaimActions.Create)]
		public async Task<IActionResult> Create()
		{
			var newRole = await this.PermissionService.GetEmptyRoleView();
			return View(newRole);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Role, ClaimActions.Create)]
		public async Task<IActionResult> Create(RoleDTO role)
		{
			var result = await RoleService.Create(role);
			return RedirectToAction("Index");
		}

		[ClaimRequirement(NavigationSections.Role, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(long? id = null)
		{
			if (id.HasValue) {
				var role = await RoleService.SelectRoleViewById(id.Value);
				if (role != null) {
					return View(role);
				}
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Role, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(RoleDTO role)
		{
			var result = await RoleService.Delete(role);
			return RedirectToAction("Index");
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Role, ClaimActions.View)]
		public async Task<IDataResult<Role>> ReadAll(ClientPageInfo clientPageInfo)
		{
			return await RoleService.ReadAll(clientPageInfo);
		}

		[ClaimRequirement(NavigationSections.Role, ClaimActions.Edit)]
		public async Task<IActionResult> Update(long? id = null)
		{
			if (!id.HasValue) {
				return NotFound();
			}
			var role = await RoleService.SelectRoleViewById(id.Value);
			if (role == null) {
				return NotFound();
			}
			return View(role);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.Role, ClaimActions.Edit)]
		public async Task<IActionResult> Update(RoleDTO role)
		{
			var result = await RoleService.Update(role);
			return RedirectToAction("Index");
		}
	}
}