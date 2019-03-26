using Common.BaseClasses;
using Microsoft.Extensions.Logging;
using Models.Base;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Business.Services.EntityServices.Base
{
	public class EntityServiceBase : LoggingWorker
	{
		protected Auditing Auditing { get; private set; }

		public EntityServiceBase(Auditing auditing, ILoggerFactory loggerFactory) : base(loggerFactory)
		{
			if (auditing == null) {
				throw new Exception("Service Base requires a non-null auditing class.");
			}
			Auditing = auditing;
		}

		public static void SetIdPostCreate<T, R>(string propertyName, T entity, R propertyValue) where T : class where R : struct
		{
			var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
			if (property != null && property.CanWrite) {
				property.SetValue(entity, propertyValue, null);
			}
		}

		public PrepareForSaveResult PrepareForSave<T, U>(T entity, long currentUserId, bool isNew = false) where T : Entity<U> where U : struct
		{
			return new SavePreparation(this.Auditing, this.Logger).PrepareForSave<T, U>(entity, currentUserId, isNew);
		}

		public PrepareForSaveResult PrepareForSave<T, U, V>(T association, long currentUserId) where T : Association<U, V> where U : struct
		{
			return new SavePreparation(this.Auditing, this.Logger).PrepareForSave<T, U, V>(association, currentUserId);
		}

		public PrepareForSaveResult PrepareForSave<T, U, V, X>(T association, long currentUserId) where T : AssociationTo3<U, V, X> where U : struct where V : struct
		{
			return new SavePreparation(this.Auditing, this.Logger).PrepareForSave<T, U, V, X>(association, currentUserId);
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U>(T entity, long currentUserId, bool creation = true) where T : Entity<U> where U : struct
		{
			var sp = new SavePreparation(this.Auditing, this.Logger);
			return await sp.PrepareForSave_Async<T, U>(entity, currentUserId, creation);
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U, V>(T association, long currentUserId, bool creation = true) where T : Association<U, V> where U : struct
		{
			var sp = new SavePreparation(this.Auditing, this.Logger);
			return await sp.PrepareForSave_Async<T, U, V>(association, currentUserId, creation);
		}

		public async Task<PrepareForSaveResult> PrepareForSave_Async<T, U, V, X>(T association, long currentUserId, bool creation = true) where T : AssociationTo3<U, V, X> where U : struct where V : struct
		{
			var sp = new SavePreparation(this.Auditing, this.Logger);
			return await sp.PrepareForSave_Async<T, U, V, X>(association, currentUserId, creation);
		}
	}
}