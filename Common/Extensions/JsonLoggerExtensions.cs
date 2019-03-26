using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Extensions {
	public static class JsonLoggerExtensions {

		public static void LogJsonError(this ILogger logger, string message, object arg) {
			logger.LogJsonError(message, new List<object> { arg });
		}

		public static void LogJsonError(this ILogger logger, string message, List<object> args) {
			logger.LogJsonError(message, args.ToArray());
		}

		public static void LogJsonError(this ILogger logger, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogError(sb.ToString());
		}

		public static void LogJsonError(this ILogger logger, Exception exception, string message, object arg) {
			logger.LogJsonError(exception, message, new List<object> { arg });
		}

		public static void LogJsonError(this ILogger logger, Exception exception, string message, List<object> args) {
			logger.LogJsonError(exception, message, args.ToArray());
		}

		public static void LogJsonError(this ILogger logger, Exception exception, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogError(exception, sb.ToString());
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, Exception exception, string message, object arg) {
			logger.LogJsonError(eventId, exception, message, new List<object> { arg });
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, Exception exception, string message, List<object> args) {
			logger.LogJsonError(eventId, exception, message, args.ToArray());
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogError(eventId, exception, sb.ToString());
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, string message, object arg) {
			logger.LogJsonError(eventId, message, new List<object> { arg });
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, string message, List<object> args) {
			logger.LogJsonError(eventId, message, args.ToArray());
		}

		public static void LogJsonError(this ILogger logger, EventId eventId, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogError(eventId, sb.ToString());
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, string message, object arg) {
			logger.LogJsonInformation(eventId, message, new List<object> { arg });
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, string message, List<object> args) {
			logger.LogJsonInformation(eventId, message, args.ToArray());
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogInformation(eventId, sb.ToString());
		}

		public static void LogJsonInformation(this ILogger logger, Exception exception, string message, object arg) {
			logger.LogJsonInformation(exception, message, new List<object> { arg });
		}

		public static void LogJsonInformation(this ILogger logger, Exception exception, string message, List<object> args) {
			logger.LogJsonInformation(exception, message, args.ToArray());
		}

		public static void LogJsonInformation(this ILogger logger, Exception exception, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogInformation(sb.ToString(), exception);
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, Exception exception, string message, object arg) {
			logger.LogJsonInformation(eventId, exception, message, new List<object> { arg });
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, Exception exception, string message, List<object> args) {
			logger.LogJsonInformation(eventId, exception, message, args.ToArray());
		}

		public static void LogJsonInformation(this ILogger logger, EventId eventId, Exception exception, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogInformation(eventId, exception, sb.ToString());
		}

		public static void LogJsonInformation(this ILogger logger, string message, object arg) {
			logger.LogJsonInformation(message, new List<object> { arg });
		}

		public static void LogJsonInformation(this ILogger logger, string message, List<object> args) {
			logger.LogJsonInformation(message, args.ToArray());
		}

		public static void LogJsonInformation(this ILogger logger, string message, params object[] args) {
			var sb = new StringBuilder();
			sb.Append(message);
			serializeArgs_AddToStringBuilder(args, sb);
			logger.LogInformation(sb.ToString());
		}

		private static void serializeArgs_AddToStringBuilder(object[] args, StringBuilder sb) {
			if (args == null || !args.Any()) {
				return;
			}
			var jsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };
			foreach (var arg in args) {
				if (arg == null) { continue; }
				var type = arg.GetType();
				var name = type.Name;
				var json = JsonConvert.SerializeObject(arg, jsonSerializerSettings);
				sb.AppendLine($"\t{ name }:\r\n\t{ json }");
			}
		}
	}
}