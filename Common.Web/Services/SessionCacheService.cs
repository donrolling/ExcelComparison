using Common.Web.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Common.Web.Services {
	public class SessionCacheService : ISessionCacheService {
		public IDistributedCache Cache { get; private set; }
		public IDataProtectionProvider DataProtectionProvider { get; private set; }
		public IHttpContextAccessor HttpContextAccessor { get; private set; }
		public IDataProtector Protector { get; private set; }

		public SessionCacheService(IDistributedCache cache, IDataProtectionProvider dataProtectionProvider, IHttpContextAccessor httpContextAccessor) {
			this.Cache = cache;
			this.HttpContextAccessor = httpContextAccessor;
			this.Protector = dataProtectionProvider.CreateProtector("SessionCacheService");
		}

		public void Clear() {
			this.HttpContextAccessor.HttpContext.Session.Clear();
		}

		public bool Exists(string key) {
			var value = this.Cache.GetString(key);
			return !string.IsNullOrEmpty(value);
		}

		public T Get<T>(string key, bool decrypt) {
			if (typeof(T).IsValueType) {
				var value = this.Cache.GetString(key);
				if (string.IsNullOrEmpty(value)) { return default(T); }
				return (T)Convert.ChangeType(value, typeof(T));
			} else {
				var stringEntity = this.Cache.GetString(key);
				if (string.IsNullOrEmpty(stringEntity)) { return default(T); }
				return decryptAndDeserialize<T>(decrypt, stringEntity);
			}
		}

		public T Get<T>(string key, T defaultValue, bool decrypt) {
			if (typeof(T).IsValueType) {
				var value = this.Cache.GetString(key);
				if (string.IsNullOrEmpty(value)) { return defaultValue; }
				return (T)Convert.ChangeType(value, typeof(T));
			} else {
				var stringEntity = this.Cache.GetString(key);
				if (string.IsNullOrEmpty(stringEntity)) { return defaultValue; }
				return decryptAndDeserialize<T>(decrypt, stringEntity);
			}
		}

		public async Task<T> GetAsync<T>(string key, bool decrypt) {
			if (typeof(T).IsValueType) {
				var value = await this.Cache.GetStringAsync(key);
				if (string.IsNullOrEmpty(value)) { return default(T); }
				return (T)Convert.ChangeType(value, typeof(T));
			} else {
				var stringEntity = await this.Cache.GetStringAsync(key);
				if (string.IsNullOrEmpty(stringEntity)) { return default(T); }
				return decryptAndDeserialize<T>(decrypt, stringEntity);
			}
		}

		public async Task<T> GetAsync<T>(string key, T defaultValue, bool decrypt) {
			if (typeof(T).IsValueType) {
				var value = await this.Cache.GetStringAsync(key);
				if (string.IsNullOrEmpty(value)) { return defaultValue; }
				return (T)Convert.ChangeType(value, typeof(T));
			} else {
				var stringEntity = await this.Cache.GetStringAsync(key);
				if (string.IsNullOrEmpty(stringEntity)) { return defaultValue; }
				return decryptAndDeserialize<T>(decrypt, stringEntity);
			}
		}

		public T GetSet<T>(string key, Func<T> func, bool encrypt) {
			var item = this.Get<T>(key, encrypt);
			if (item == null) {
				item = func.Invoke();
				//don't cache null stuff
				if (item == null) { return item; }
				this.Set<T>(key, item, encrypt);
			}
			return item;
		}

		public async Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, bool encrypt) {
			var item = await this.GetAsync<T>(key, encrypt);
			var defaultValue = default(T);
			//todo: not sure if the defaultValue is going to work here.
			if (item != null && !item.Equals(defaultValue)) { return item; }
			item = await func.Invoke();
			//don't cache null stuff
			if (item == null) { return item; }
			await this.SetAsync<T>(key, item, encrypt);
			return item;
		}

		public void Remove(string key) {
			this.Cache.Remove(key);
		}

		public async Task RemoveAsync(string key) {
			await this.Cache.RemoveAsync(key);
		}

		public void Set<T>(string key, T entity, bool encrypt) {
			if (entity.GetType().IsValueType) {
				encryptAndCache(key, entity.ToString(), encrypt);
			} else {
				var objectBox = JsonConvert.SerializeObject(entity);
				encryptAndCache(key, objectBox, encrypt);
			}
		}

		public async Task SetAsync<T>(string key, T entity, bool encrypt) {
			if (entity.GetType().IsValueType) {
				await encryptAndCacheAsync(key, entity.ToString(), encrypt);
			} else {
				var objectBox = JsonConvert.SerializeObject(entity);
				await encryptAndCacheAsync(key, objectBox, encrypt);
			}
		}

		private T decryptAndDeserialize<T>(bool decrypt, string stringEntity) {
			if (!decrypt) {
				return JsonConvert.DeserializeObject<T>(stringEntity);
			} else {
				var decryptedValue = this.Protector.Unprotect(stringEntity);
				return JsonConvert.DeserializeObject<T>(decryptedValue);
			}
		}

		private void encryptAndCache(string key, string value, bool encrypt) {
			if (!encrypt) {
				this.Cache.SetString(key, value);
			} else {
				var encValue = this.Protector.Protect(value);
				this.Cache.SetString(key, encValue);
			}
		}

		private async Task encryptAndCacheAsync(string key, string value, bool encrypt) {
			if (!encrypt) {
				await this.Cache.SetStringAsync(key, value);
			} else {
				var encValue = this.Protector.Protect(value);
				await this.Cache.SetStringAsync(key, encValue);
			}
		}
	}
}