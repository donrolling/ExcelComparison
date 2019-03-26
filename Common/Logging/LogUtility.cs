using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Common.Logging {
	public class LogUtility {

		public static ILogger GetLogger<T>(IServiceProvider serviceProvider) where T : class {
			var type = typeof(T);
			return getLogger(serviceProvider, type);
		}

		public static ILogger GetLogger(IServiceProvider serviceProvider, Type type) {
			return getLogger(serviceProvider, type);
		}

		public static ILogger GetLogger(ILoggerFactory loggerFactory, Type type) {
			return getLoggerFromFactory(type, loggerFactory);
		}

		private static ILogger getLogger(IServiceProvider serviceProvider, Type type) {
			var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
			return getLoggerFromFactory(type, loggerFactory);
		}

		private static ILogger getLoggerFromFactory(Type type, ILoggerFactory loggerFactory) {
			var name = type.Name;
			var _namespace = type.Namespace;
			return loggerFactory.CreateLogger($"{ _namespace }.{ name }");
		}
	}
}