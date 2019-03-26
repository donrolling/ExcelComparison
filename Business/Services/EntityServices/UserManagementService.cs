using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Data.Dapper.Models;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Models.Base;
using Models.DTO;
using Models.Entities;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.EntityServices
{
	public class UserManagementService : UserService, IUserManagementService
	{
		public IMembershipService MembershipService { get; }
		public IRoleService RoleService { get; }
		public IUserRoleService UserRoleService { get; }

		private static readonly Auditing _auditing = new Auditing();

		public UserManagementService(IMembershipService membershipService, IUserRepository userRespository, IUserRoleService userRoleService, IRoleService roleService, ILoggerFactory loggerFactory) : base(userRespository, loggerFactory)
		{
			MembershipService = membershipService;
			UserRoleService = userRoleService;
			RoleService = roleService;
		}

		public override async Task<TransactionResponse> Delete(long id){
			var deleteResult = await UserRoleService.DeleteByUserId(id);
			if (deleteResult.Failure) {
				return deleteResult;
			}
			return await base.Delete(id);
		}

		public async Task<InsertResponse<long>> Create(UpdateUserDTO updateUserDTO)
		{
			var currentUserId = await this.MembershipService.CurrentUserId();
			var createResult = await this.Create(updateUserDTO, currentUserId);
			if (createResult.Failure) {
				return createResult;
			}
			var addRoleResult = await UserRoleService.Update(createResult.Id, updateUserDTO.RoleIds);
			if (addRoleResult.Failure) {
				return InsertResponse<long>.GetInsertResponse(addRoleResult);
			}
			return createResult;
		}

		public async Task<UpdateUserDTO> SelectEmptyUserView()
		{
			var userDTO = new UpdateUserDTO();
			var allRoles = await RoleService.ReadAll();
			var roles = allRoles.ToList();
			userDTO.AllRoles = allRoles.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
			userDTO.AvailableRoles = getRoleList(roles);
			userDTO.SelectedRoles = new List<SelectListItem>();
			return userDTO;
		}

		public async Task<UpdateUserDTO> SelectUserViewById(long userId)
		{
			var user = await this.UserRepository.SelectById(userId);
			if (user == null) {
				return null;
			}
			var userDTO = new UpdateUserDTO();
			userDTO.InjectFrom(user);
			var roles = await UserRoleService.SelectByUserId(userDTO.Id);
			userDTO.Roles = roles.ToList();
			userDTO.RoleIds = roles.Select(x => x.RoleId).ToArray();
			var allRoles = await RoleService.ReadAll();
			var availableRoles = getRoleList(allRoles, userDTO.Roles);
			userDTO.AllRoles = allRoles.Select(x => new SelectListItem(x.Name, x.Id.ToString()));
			userDTO.AvailableRoles = availableRoles;
			userDTO.SelectedRoles = userDTO.Roles.Select(x => new SelectListItem(x.RoleName, x.RoleId.ToString()));
			return userDTO;
		}

		public async Task<TransactionResponse> Update(UpdateUserDTO updateUserDTO)
		{
			var currentUserId = await this.MembershipService.CurrentUserId();
			var updateResult = await this.Update(updateUserDTO, currentUserId);
			if (updateResult.Failure) {
				return updateResult;
			}
			var deleteResult = await this.UserRoleService.DeleteByUserId(updateUserDTO.Id);
			if (deleteResult.Failure) {
				return deleteResult;
			}
			var addRoleResult = await UserRoleService.Update(updateUserDTO.Id, updateUserDTO.RoleIds);
			if (addRoleResult.Failure) {
				return addRoleResult;
			}
			return updateResult;
		}

		private IEnumerable<SelectListItem> getRoleList(IEnumerable<Role> allRoles, IEnumerable<UserRoleDTO> userRoles = null)
		{
			var selectItemList = allRoles.Select(a => new SelectListItem(a.Name, a.Id.ToString()));
			if (userRoles != null && userRoles.Any()) {
				selectItemList = selectItemList.Where(x => !userRoles.Where(y => x.Value == y.RoleId.ToString()).Any()).ToList();
			}
			return selectItemList;
		}
	}
}