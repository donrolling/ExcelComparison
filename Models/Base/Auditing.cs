using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace Models.Base {
	public class Auditing {
		/// <summary>
		/// Makes sure that everybody is using UTC
		/// </summary>
		private static DateTime Now {
			get {
				return DateTime.UtcNow;
			}
		}
		public string _createdById = "CreatedById";
		public string _createdDate = "CreatedDate";
		public string _isActive = "IsActive";
		public string _updatedById = "UpdatedById";
		public string _updatedDate = "UpdatedDate";
		private static ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo> KeyPropertiesCache = new ConcurrentDictionary<RuntimeTypeHandle, PropertyInfo>();

		public Auditing() {
		}

		public Auditing(string createdDate, string updatedDate, string createdById, string updatedById, string isActive, string isDeleted) {
			_createdDate = string.IsNullOrEmpty(createdDate) ? _createdDate : createdDate;
			_updatedDate = string.IsNullOrEmpty(updatedDate) ? _updatedDate : updatedDate;
			_createdById = string.IsNullOrEmpty(createdById) ? _createdById : createdById;
			_updatedById = string.IsNullOrEmpty(updatedById) ? _updatedById : updatedById;
			_isActive = string.IsNullOrEmpty(isActive) ? _isActive : isActive;
		}

		/// <summary>
		/// This is a little hacky, because we convert down to string for comparison.
		/// It does, however, work for every scenario that I can think of.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool IsEntityNew<T, U>(T entity) where T : BaseEntity<U> where U : struct {
			var defaultValue = default(U).ToString();
			if (defaultValue == entity.Id.ToString()) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Sets IsActive, IsDeleted, CreatedById, CreatedDate, UpdatedDate and UpdatedById for the proper circumstances.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity"></param>
		/// <param name="currentUserId"></param>
		/// <param name="isNew">Allows new items to be treated as such even if they do not have a default value applied to their key.</param>
		public bool SetAuditFields<T, U>(T entity, long currentUserId, bool isNew = false) where T : BaseEntity<U> where U : struct {
			var createdDateValue = readValue<T, DateTime>(entity, _createdDate);
			var isCreatedDateNew = createdDateValue.Equals(default(DateTime));
			var isEntityIdNew = IsEntityNew<T, U>(entity);
			var now = Now;
			if (isCreatedDateNew) {
				SetNonOverrideableProperty<T, DateTime>(_createdDate, entity, now);
			}
			if (isNew || isEntityIdNew) {
				SetNonOverrideableProperty<T, bool>(_isActive, entity, true);
			}
			SetNonOverrideableProperty<T, long>(_createdById, entity, currentUserId);
			SetOverrideableProperty<T, DateTime>(_updatedDate, entity, now);
			SetOverrideableProperty<T, long>(_updatedById, entity, currentUserId);
			return isNew;
		}

		public void SetAuditFields<T, U, V, X>(T association, long currentUserId)
			where T : AssociationTo3<U, V, X>
			where U : struct
			where V : struct {
			var now = Now;
			var createdDateValue = readValue<T, DateTime>(association, _createdDate);
			var isCreatedDateNew = createdDateValue.Equals(default(DateTime));
			if (isCreatedDateNew) {
				SetNonOverrideableProperty<T, bool>(_isActive, association, true);
			}
			SetNonOverrideableProperty<T, DateTime>(_createdDate, association, now);
			SetOverrideableProperty<T, DateTime>(_updatedDate, association, now);
			SetNonOverrideableProperty<T, long>(_createdById, association, currentUserId);
			SetOverrideableProperty<T, long>(_updatedById, association, currentUserId);
		}

		/// <summary>
		/// Alway attempts to write values into all audit fields. Will not overwrite values that are already set, with the exception of Updated Date and Update By.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="association"></param>
		/// <param name="currentUserId"></param>
		/// <param name="isNew"></param>
		public void SetAuditFields<T, U, V>(T association, long currentUserId) where T : Association<U, V> where U : struct {
			var now = Now;
			var createdDateValue = readValue<T, DateTime>(association, _createdDate);
			var isCreatedDateNew = createdDateValue.Equals(default(DateTime));
			if (isCreatedDateNew) {
				SetNonOverrideableProperty<T, bool>(_isActive, association, true);
			}
			SetNonOverrideableProperty<T, DateTime>(_createdDate, association, now);
			SetOverrideableProperty<T, DateTime>(_updatedDate, association, now);
			SetNonOverrideableProperty<T, long>(_createdById, association, currentUserId);
			SetOverrideableProperty<T, long>(_updatedById, association, currentUserId);
		}

		//this doesn't work very well for boolean values
		public void SetNonOverrideableProperty<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			setNonOverrideableProperty(propertyName, entity, propertyValue);
		}

		public async Task<bool> SetNonOverrideablePropertyAsync<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			return await Task.Run(() => setNonOverrideableProperty(propertyName, entity, propertyValue));
		}

		public void SetOverrideableProperty<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			setOverrideableProperty<T, R>(propertyName, entity, propertyValue);
		}

		public async Task<bool> SetOverrideablePropertyAsync<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			return await Task.Run(() => setOverrideableProperty<T, R>(propertyName, entity, propertyValue));
		}

		private R readValue<T, R>(object association, string propertyName) where T : class where R : struct {
			var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property == null) {
				return default(R);
			}
			var obj = property.GetValue(association);
			if (obj == null) {
				return default(R);
			}
			return (R)obj;
		}

		private bool setNonOverrideableProperty<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			if (entity == null) {
				return false;
			}
			var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property == null || !property.CanWrite) {
				return false;
			}
			var obj = property.GetValue(entity);
			if (obj == null) {
				property.SetValue(entity, propertyValue, null);
				return false;
			}
			R existingValue = (R)obj;
			R defaultValue = default(R);
			if (existingValue.Equals(null)) {
				return false;
			}
			if (existingValue.Equals(defaultValue)) {//if value isn't really set, then set it
				property.SetValue(entity, propertyValue, null);
			}
			return true;
		}

		private bool setOverrideableProperty<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct {
			if (entity == null) {
				return false;
			}
			var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property != null && property.CanWrite) {
				property.SetValue(entity, propertyValue, null);
			}
			return true;
		}
	}
}