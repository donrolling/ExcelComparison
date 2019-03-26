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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services.EntityServices
{
	public class PermissionService : PermissionBaseService, IPermissionService
	{
		public IEntityDapperRepository<Permission, long> EntityRepository { get { return this.PermissionRepository; } }

		public IRolePermissionService RolePermissionService { get; }

		public PermissionService(IMembershipService membershipService, IPermissionRepository permissionRepository, ILoggerFactory loggerFactory, IRolePermissionService rolePermissionService) : base(membershipService, permissionRepository, loggerFactory)
		{
			RolePermissionService = rolePermissionService;
		}

		public async Task<TransactionResponse> Delete(Permission permission)
		{
			var deleteRolePermissionResult  = await RolePermissionService.DeleteByPermissionId(permission.Id);
			if (deleteRolePermissionResult.Failure) {
				return deleteRolePermissionResult;
			}
			var deletePermissionResult = await Delete(permission.Id);
			return deleteRolePermissionResult;
		}

		public async Task<RoleDTO> GetEmptyRoleView()
		{
			var roleDTO = new RoleDTO();
			var allPermissions = await this.ReadAll();
			var availablePermissions = GetPermissionList(allPermissions);
			roleDTO.AllPermissions = allPermissions.Select(x => new SelectListItem($"{x.Name} - {x.Action}", x.Id.ToString()));
			roleDTO.AvailablePermissions = availablePermissions;
			roleDTO.SelectedPermissions = new List<SelectListItem>();
			return roleDTO;
		}

		public IEnumerable<SelectListItem> GetPermissionList(IEnumerable<Permission> allPermissions, IEnumerable<RolePermissionDTO> rolePermissions = null)
		{
			var selectItemList = allPermissions.Select(a => new SelectListItem($"{ a.Name } - { a.Action }", a.Id.ToString()));
			if (rolePermissions != null && rolePermissions.Any()) {
				selectItemList = selectItemList.Where(x => !rolePermissions.Where(y => x.Value == y.PermissionId.ToString()).Any()).ToList();
			}
			return selectItemList;
		}

		public async Task<IEnumerable<Permission>> ReadAll()
		{
			return await PermissionRepository.ReadAll();
		}
	}
}