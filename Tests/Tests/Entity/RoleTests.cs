using AutoFixture;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class RoleIntegrationTests : TestBase {
		public IRoleService RoleService { get; private set; }

		public RoleIntegrationTests() {
			this.RoleService = this.ServiceProvider.GetService<IRoleService>();
		}

		[TestMethod]
		public async Task CRUD_Role_GivenValidValues_Succeeds() {
			var fixture = new Fixture();
			var role = fixture.Build<Role>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			Assert.IsNotNull(role);

			//create object
			var createResult = await this.RoleService.Create(role);
			Assert.IsTrue(createResult.Success);

			try {
				//select object by id to ensure that it was saved to db
				var newRole = await this.RoleService.SelectById(createResult.Id);
				Assert.IsNotNull(newRole);

				//update object to ensure that it can be modified and saved to db
				newRole.Name = "Something Random";

				//update the item in the database
				var updateResult = await this.RoleService.Update(newRole);
				Assert.IsTrue(updateResult.Success);

				//verify that the data in the newly updated object is not the same as it was previously.
				var postUpdatedRole = this.RoleService.SelectById(createResult.Id);
				Assert.IsNotNull(postUpdatedRole);
				Assert.AreNotEqual(role.Name, newRole.Name);
			} finally {
				//delete the item in the database
				var deleteResult = await this.RoleService.Delete(createResult.Id);
				Assert.IsTrue(deleteResult.Success);

				//verify that the item was deleted
				var deleteConfirmRole = this.RoleService.SelectById(createResult.Id);
				Assert.IsNull(deleteConfirmRole.Result);
			}
		}
	}
}