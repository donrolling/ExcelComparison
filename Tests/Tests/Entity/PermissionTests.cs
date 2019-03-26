using AutoFixture;
using Business.Service.EntityServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using System.Threading.Tasks;
using Tests.Models;

namespace Tests.Tests.Entity {
	[TestClass]
	public class PermissionIntegrationTests : TestBase {
		public IPermissionService PermissionService { get; private set; }

		public PermissionIntegrationTests() {
			this.PermissionService = this.ServiceProvider.GetService<IPermissionService>();
		}

		[TestMethod]
		public async Task CRUD_Permission_GivenValidValues_Succeeds() {
			var fixture = new Fixture();
			var permission = fixture.Build<Permission>()
				.Without(a => a.Id)
				.Without(a => a.CreatedDate)
				.Without(a => a.CreatedById)
				.Without(a => a.UpdatedDate)
				.Without(a => a.UpdatedById)
				.Create();
			Assert.IsNotNull(permission);

			//create object
			var createResult = await this.PermissionService.Create(permission);
			Assert.IsTrue(createResult.Success);

			try {
				//select object by id to ensure that it was saved to db
				var newPermission = await this.PermissionService.SelectById(createResult.Id);
				Assert.IsNotNull(newPermission);

				//update object to ensure that it can be modified and saved to db
				newPermission.Name = "Something Random";

				//update the item in the database
				var updateResult = await this.PermissionService.Update(newPermission);
				Assert.IsTrue(updateResult.Success);

				//verify that the data in the newly updated object is not the same as it was previously.
				var postUpdatedPermission = this.PermissionService.SelectById(createResult.Id);
				Assert.IsNotNull(postUpdatedPermission);
				Assert.AreNotEqual(permission.Name, newPermission.Name);
			} finally {
				//delete the item in the database
				var deleteResult = await this.PermissionService.Delete(createResult.Id);
				Assert.IsTrue(deleteResult.Success);

				//verify that the item was deleted
				var deleteConfirmPermission = this.PermissionService.SelectById(createResult.Id);
				Assert.IsNull(deleteConfirmPermission.Result);
			}
		}
	}
}