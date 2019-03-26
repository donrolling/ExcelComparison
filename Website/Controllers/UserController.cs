using Business.Security;
using Business.Security.WebAPI;
using Business.Service.EntityServices.Interfaces;
using Business.Services.Membership;
using Data.Dapper.Models;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Entities;
using Models.Interfaces;
using System.Threading.Tasks;

namespace Website.Controllers
{
	[ServiceFilter(typeof(Website_SecurityFilter))]
	[ClaimRequirement(NavigationSections.User, ClaimActions.View)]
	public class UserController : Controller
	{
		public IUserManagementService UserManagementService { get; }

		public UserController(IUserManagementService userManagementService)
		{
			UserManagementService = userManagementService;
		}

		[ClaimRequirement(NavigationSections.User, ClaimActions.Create)]
		public async Task<IActionResult> Create()
		{
			var userDTO = await this.UserManagementService.SelectEmptyUserView();
			return View(userDTO);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.User, ClaimActions.Create)]
		public async Task<IActionResult> Create(UpdateUserDTO user)
		{
			var result = await this.UserManagementService.Create(user);
			return RedirectToAction("Index");
		}

		[ClaimRequirement(NavigationSections.User, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(long? id = null)
		{
			if (id.HasValue) {
				var user = await UserManagementService.SelectUserViewById(id.Value);
				if (user != null) {
					return View(user);
				}
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.User, ClaimActions.Delete)]
		public async Task<IActionResult> Delete(long id)
		{
			var result = await this.UserManagementService.Delete(id);
			return RedirectToAction("Index");
		}

		[ClaimRequirement(NavigationSections.User, ClaimActions.View)]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.User, ClaimActions.View)]
		public async Task<IDataResult<User>> ReadAll(ClientPageInfo clientPageInfo)
		{
			return await UserManagementService.ReadAll(clientPageInfo);
		}

		[ClaimRequirement(NavigationSections.User, ClaimActions.Edit)]
		public async Task<IActionResult> Update(long? id = null)
		{
			if (!id.HasValue) {
				return NotFound();
			}
			var userDTO = await UserManagementService.SelectUserViewById(id.Value);
			if (userDTO == null) {
				return NotFound();
			}
			return View(userDTO);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ClaimRequirement(NavigationSections.User, ClaimActions.Edit)]
		public async Task<IActionResult> Update(UpdateUserDTO user)
		{
			var result = await UserManagementService.Update(user);
			return RedirectToAction("Index");
		}
	}
}