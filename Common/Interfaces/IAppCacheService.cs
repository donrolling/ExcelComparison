using System;
using System.Threading.Tasks;

namespace Common.Interfaces {
	public interface IAppCacheService {

		T Get<T>(string key);

		T Get<T>(string key, T defaultValue);

		Task<T> GetAsync<T>(string key);

		Task<T> GetAsync<T>(string key, T defaultValue);

		T GetSet<T>(string key, Func<T> func, int expireTimeInMinutes = 20);

		Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, int expireTimeInMinutes = 20);

		void Remove(string key);

		Task RemoveAsync(string key);

		void Set<T>(string key, T entity, int expireTimeInMinutes = 20);

		Task SetAsync<T>(string key, T entity, int expireTimeInMinutes = 20);
	}
}