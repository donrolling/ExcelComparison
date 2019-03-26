using Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services {
	public class AppCacheService : IAppCacheService {
		public IMemoryCache Cache { get; private set; }

		public AppCacheService(IMemoryCache cache) {
			this.Cache = cache;
		}

		public T Get<T>(string key) {
			object obj = this.Cache.Get<T>(key);
			if (obj != null) {
				return (T)obj;
			}
			return default(T);
		}

		public T Get<T>(string key, T defaultValue) {
			object obj = this.Cache.Get<T>(key);
			if (obj != null) {
				return (T)obj;
			}
			return defaultValue;
		}

		public async Task<T> GetAsync<T>(string key) {
			return await new Task<T>(() => this.Get<T>(key));
		}

		public async Task<T> GetAsync<T>(string key, T defaultValue) {
			return await new Task<T>(() => this.Get<T>(key, defaultValue));
		}

		public T GetSet<T>(string key, Func<T> func, int expireTimeInMinutes = 20) {
			var item = this.Get<T>(key);
			if (item == null) {
				item = func.Invoke();
				if (item == null) {
					return item;
				}

				Set<T>(key, item, expireTimeInMinutes);
			}
			return item;
		}

		public async Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, int expireTimeInMinutes = 20) {
			var item = this.Get<T>(key);
			var defaultValue = default(T);
			//todo: not sure if the defaultValue is going to work here.
			if (item != null && !item.Equals(defaultValue)) {
				return item;
			}
			item = await func.Invoke();
			if (item == null) {
				//don't cache null stuff
				return item;
			}
			await SetAsync<T>(key, item, expireTimeInMinutes);
			return item;
		}

		public void Remove(string key) {
			this.Cache.Remove(key);
		}

		public async Task RemoveAsync(string key) {
			await Task.Run(() => this.Remove(key));
		}

		public void Set<T>(string key, T entity, int expireTimeInMinutes = 20) {
			var expireTime = DateTime.UtcNow.AddMinutes(expireTimeInMinutes);
			this.Cache.Set<T>(key, entity, expireTime);
		}

		public async Task SetAsync<T>(string key, T entity, int expireTimeInMinutes = 20) {
			await Task.Run(() => {
				var expireTime = DateTime.UtcNow.AddMinutes(expireTimeInMinutes);
				this.Cache.Set<T>(key, entity, expireTime);
			});
		}
	}
}