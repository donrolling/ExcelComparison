using AutoFixture;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class RolePermissionIntegrationTests : TestBase {
		public IPermissionService PermissionService { get; private set; }
		public IRolePermissionService RolePermissionService { get; private set; }
		public IRoleService RoleService { get; private set; }

		public RolePermissionIntegrationTests() {
			this.PermissionService = this.ServiceProvider.GetService<IPermissionService>();
			this.RolePermissionService = this.ServiceProvider.GetService<IRolePermissionService>();
			this.RoleService = this.ServiceProvider.GetService<IRoleService>();
		}

		[TestMethod]
		public async Task CRUD_RolePermission_GivenValidValues_Succeeds() {
			var fixture = new Fixture();
			var role = fixture.Build<Role>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			var createRoleResult = await this.RoleService.Create(role);
			Assert.IsTrue(createRoleResult.Success);

			var user = fixture.Build<Permission>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			var createPermissionResult = await this.PermissionService.Create(user);
			Assert.IsTrue(createPermissionResult.Success);

			var rolePermission = new RolePermission { RoleId = createRoleResult.Id, PermissionId = createPermissionResult.Id };
			var createRolePermissionResult = await this.RolePermissionService.Create(rolePermission);
			Assert.IsTrue(createRolePermissionResult.Success);

			try {
				//select object by id to ensure that it was saved to db
				var newRolePermission = await this.RolePermissionService.SelectById(createRoleResult.Id, createPermissionResult.Id);
				Assert.IsNotNull(newRolePermission);
			} finally {
				var deleteRolePermissionResult = await this.RolePermissionService.Delete(createRoleResult.Id, createPermissionResult.Id);
				Assert.IsTrue(deleteRolePermissionResult.Success);

				var deleteRoleResult = await this.RoleService.Delete(createRoleResult.Id);
				Assert.IsTrue(deleteRoleResult.Success);

				var deletePermissionResult = await this.PermissionService.Delete(createPermissionResult.Id);
				Assert.IsTrue(deletePermissionResult.Success);

				var rolePermissionSelectResult = await this.RolePermissionService.SelectById(createRoleResult.Id, createPermissionResult.Id);
				Assert.IsNull(rolePermissionSelectResult);
			}
		}
	}
}