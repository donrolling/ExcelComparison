using Common.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Base {
	public class SavePreparation {
		public ILogger Logger { get; private set; }
		protected Auditing Auditing { get; private set; }
		/// <summary>
		/// Makes sure that everybody is using UTC
		/// </summary>
		private DateTime Now;

		public SavePreparation(Auditing auditing, ILogger logger) {
			this.Auditing = auditing;
			Logger = logger;
			this.Now = DateTime.UtcNow;
		}

		/// <summary>
		/// Checks class for validity via DataAnnotations and sets Audit Fields
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="currentUserId"></param>
		/// <param name="isNew"></param>
		/// <returns>
		///		bool, isNew
		///		bool, isValid
		///		string, validityMessage
		/// </returns>
		public PrepareForSaveResult PrepareForSave<T, U>(T entity, long currentUserId, bool isNew = false) where T : BaseEntity<U> where U : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be empty.");
			}
			var auditResult = this.Auditing.SetAuditFields<T, U>(entity, currentUserId, isNew);
			if (entity.IsValid()) {
				return new PrepareForSaveResult { IsNew = auditResult, IsValid = true, ValidationMessage = string.Empty };
			}
			var validationResult = validateObject(entity);
			return validationResult;
		}

		public PrepareForSaveResult PrepareForSave<T, U, V>(T association, long currentUserId) where T : Association<U, V> where U : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be zero.");
			}
			if (association.IsValid()) {
				this.Auditing.SetAuditFields<T, U, V>(association, currentUserId);
				//isnew is always false for associations
				return new PrepareForSaveResult { IsNew = false, IsValid = true, ValidationMessage = string.Empty };
			}
			var validationResult = validateObject(association);
			return validationResult;
		}

		public PrepareForSaveResult PrepareForSave<T, U, V, X>(T association, long currentUserId) where T : AssociationTo3<U, V, X> where U : struct where V : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be zero.");
			}
			if (association.IsValid()) {
				this.Auditing.SetAuditFields<T, U, V, X>(association, currentUserId);
				//isnew is always false for associations
				return new PrepareForSaveResult { IsNew = false, IsValid = true, ValidationMessage = string.Empty };
			}
			var validationResult = validateObject(association);
			return validationResult;
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U>(T entity, long currentUserId, bool creation = true) where T : BaseEntity<U> where U : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be zero.");
			}
			var isNew = await this.getPrepareForSaveTaskResult<T>(entity, currentUserId, creation);
			if (!entity.IsValid()) {
				var validationResult = validateObject(entity);
				return validationResult;
			}
			return new PrepareForSaveResult { IsNew = isNew, IsValid = true, ValidationMessage = string.Empty };
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U, V>(T association, long currentUserId, bool creation = true) where T : Association<U, V> where U : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be zero.");
			}
			var isNew = await this.getPrepareForSaveTaskResult<T>(association, currentUserId, creation);
			if (!association.IsValid()) {
				var validationResult = validateObject(association);
				return validationResult;
			}
			//isnew is always false for associations
			return new PrepareForSaveResult { IsNew = false, IsValid = true, ValidationMessage = string.Empty };
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U, V, X>(T association, long currentUserId, bool creation = true) where T : AssociationTo3<U, V, X> where U : struct where V : struct {
			if (currentUserId == 0) {
				throw new Exception("PrepareForSave: CurrentUserId cannot be zero.");
			}
			var isNew = await this.getPrepareForSaveTaskResult<T>(association, currentUserId, creation);
			if (!association.IsValid()) {
				var validationResult = validateObject(association);
				return validationResult;
			}
			//isnew is always false for associations
			return new PrepareForSaveResult { IsNew = false, IsValid = true, ValidationMessage = string.Empty };
		}

		private async Task<bool> getPrepareForSaveTaskResult<T>(T entity, long currentUserId, bool creation) where T : class {
			var tasks = new List<Task<bool>>();
			if (creation) {
				tasks.Add(this.Auditing.SetNonOverrideablePropertyAsync(this.Auditing._createdDate, entity, Now));
				tasks.Add(this.Auditing.SetNonOverrideablePropertyAsync(this.Auditing._createdById, entity, currentUserId));
			}
			tasks.Add(this.Auditing.SetOverrideablePropertyAsync(this.Auditing._updatedDate, entity, Now));
			tasks.Add(this.Auditing.SetOverrideablePropertyAsync(this.Auditing._updatedById, entity, currentUserId));
			var results = await Task.WhenAll(tasks);
			return !results.Any(a => a == false);
		}

		private PrepareForSaveResult validateObject(object entity) {
			var errors = entity.GetValidationErrors();
			if (errors == null) {
				return new PrepareForSaveResult { IsNew = false, IsValid = true };
			}
			var validationMessage = new StringBuilder();
			validationMessage.AppendLine("Errors have been found while attempting to save:");
			foreach (var error in errors) {
				var memberNames = error.MemberNames.ToList();
				memberNames.ForEach(a => a = a.UnCamelCase());
				validationMessage.AppendLine(memberNames.Aggregate((accum, next) => string.Concat(accum, "\r\n", next)));
				validationMessage.Append(error.ErrorMessage);
			}
			Logger.LogError(validationMessage.ToString());
			return new PrepareForSaveResult { IsNew = false, IsValid = false, ValidationMessage = validationMessage.ToString() };
		}
	}
}