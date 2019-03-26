using System;
using System.Threading.Tasks;

namespace Common.Web.Interfaces {
	public interface ISessionCacheService {

		void Clear();

		bool Exists(string key);

		T Get<T>(string key, bool decrypt);

		T Get<T>(string key, T defaultValue, bool decrypt);

		Task<T> GetAsync<T>(string key, bool decrypt);

		Task<T> GetAsync<T>(string key, T defaultValue, bool encrypt);

		T GetSet<T>(string key, Func<T> func, bool encrypt);

		Task<T> GetSetAsync<T>(string key, Func<Task<T>> func, bool encrypt);

		void Remove(string key);

		Task RemoveAsync(string key);

		void Set<T>(string key, T entity, bool encrypt);

		Task SetAsync<T>(string key, T entity, bool encrypt);
	}
}