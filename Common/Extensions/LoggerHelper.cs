using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions {
	public static class LoggerHelper {

		public static void LogInformation(this ILogger logger, object obj, string message) {
			var objString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
			var completeMessage = $"{ message } | Object:\r\n{ objString }";
			logger.LogInformation(completeMessage);
		}
	}
}