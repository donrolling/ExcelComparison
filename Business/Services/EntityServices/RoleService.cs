using Business.Interfaces;
using Business.Service.EntityServices.Interfaces;
using Business.Services.EntityServices.BaseServices;
using Data.Dapper.Models;
using Data.Interfaces;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Models.Entities;
using Omu.ValueInjecter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.EntityServices
{
	public class RoleService : RoleBaseService, IRoleService
	{
		public IEntityDapperRepository<Role, long> EntityRepository { get { return this.RoleRepository; } }

		public IPermissionService PermissionService { get; }
		public IRolePermissionService RolePermissionService { get; }
		public IUserRoleService UserRoleService { get; }

		public RoleService(IMembershipService membershipService, IPermissionService permissionService, IRoleRepository roleRepository, ILoggerFactory loggerFactory, IRolePermissionService rolePermissionService, IUserRoleService userRoleService) : base(membershipService, roleRepository, loggerFactory)
		{
			PermissionService = permissionService;
			RolePermissionService = rolePermissionService;
			UserRoleService = userRoleService;
		}

		public async Task<InsertResponse<long>> Create(RoleDTO roleDTO)
		{
			var createResult = await base.Create(roleDTO);
			if (createResult.Failure) {
				return createResult;
			}
			var createRolePermissionResult = await RolePermissionService.Create(createResult.Id, roleDTO.PermissionIds);
			if (createRolePermissionResult.Failure) {
				return InsertResponse<long>.GetInsertResponse(createRolePermissionResult);
			}
			return createResult;
		}

		public async Task<TransactionResponse> Delete(RoleDTO roleDTO)
		{
			var deleteRolePermissionResult = await RolePermissionService.DeleteByRoleId(roleDTO.Id);
			if (deleteRolePermissionResult.Failure) {
				return deleteRolePermissionResult;
			}
			var deleteUserRoleResult = await UserRoleService.DeleteByRoleId(roleDTO.Id);
			if (deleteUserRoleResult.Failure) {
				return deleteUserRoleResult;
			}
			var deleteRoleResult = await Delete(roleDTO.Id);
			return deleteRoleResult;
		}

		public async Task<IEnumerable<Role>> ReadAll()
		{
			return await RoleRepository.ReadAll();
		}

		public async Task<RoleDTO> SelectRoleViewById(long roleId)
		{
			var role = await SelectById(roleId);
			if (role == null) {
				return null;
			}

			var roleDTO = new RoleDTO();
			roleDTO.InjectFrom(role);

			var permissions = await RolePermissionService.SelectByRoleId(roleId);
			if (permissions == null) {
				return roleDTO;
			}
			roleDTO.Permissions = permissions;
			roleDTO.PermissionIds = permissions.Select(x => x.PermissionId).ToArray();

			var allPermissions = await this.PermissionService.ReadAll();
			var availablePermissions = this.PermissionService.GetPermissionList(allPermissions, permissions);
			roleDTO.AllPermissions = allPermissions.Select(x => new SelectListItem($"{x.Name} - {x.Action}", x.Id.ToString()));
			roleDTO.AvailablePermissions = availablePermissions;
			roleDTO.SelectedPermissions = permissions.Select(x => new SelectListItem($"{x.PermissionName} - {x.Action}", x.PermissionId.ToString()));

			return roleDTO;
		}

		public async Task<TransactionResponse> Update(RoleDTO roleDTO)
		{
			var result = await base.Update(roleDTO);
			if (result.Failure) {
				return result;
			}				
			var udpateResult = await RolePermissionService.Update(roleDTO.Id, roleDTO.PermissionIds);
			return udpateResult;
		}
	}
}