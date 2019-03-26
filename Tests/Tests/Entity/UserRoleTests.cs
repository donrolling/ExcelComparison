using AutoFixture;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class UserRoleIntegrationTests : TestBase {
		public IRoleService RoleService { get; private set; }
		public IUserRoleService UserRoleService { get; private set; }
		public IUserService UserService { get; }

		public UserRoleIntegrationTests() {
			this.UserRoleService = this.ServiceProvider.GetService<IUserRoleService>();
			this.RoleService = this.ServiceProvider.GetService<IRoleService>();
			this.UserService = this.ServiceProvider.GetService<IUserService>();
		}

		[TestMethod]
		public async Task CRUD_UserRole_GivenValidValues_Succeeds() {
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

			var user = fixture.Build<User>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			var createUserResult = await this.UserService.Create(user, this.MembershipService.CurrentUserId().Result);
			Assert.IsTrue(createUserResult.Success);

			var userRole = new UserRole { UserId = createUserResult.Id, RoleId = createRoleResult.Id };
			var createUserRoleResult = await this.UserRoleService.Create(userRole);
			Assert.IsTrue(createUserRoleResult.Success);

			try {
				//select object by id to ensure that it was saved to db
				var newUserRole = await this.UserRoleService.SelectById(createUserResult.Id, createRoleResult.Id);
				Assert.IsNotNull(newUserRole);
			} finally {
				var deleteUserRoleResult = await this.UserRoleService.Delete(createUserResult.Id, createRoleResult.Id);
				Assert.IsTrue(deleteUserRoleResult.Success);

				var deleteRoleResult = await this.RoleService.Delete(createRoleResult.Id);
				Assert.IsTrue(deleteRoleResult.Success);

				var deleteUserResult = await this.UserService.Delete(createUserResult.Id);
				Assert.IsTrue(deleteUserResult.Success);

				var rolePermissionSelectResult = await this.UserRoleService.SelectById(createUserResult.Id, createRoleResult.Id);
				Assert.IsNull(rolePermissionSelectResult);
			}
		}
	}
}