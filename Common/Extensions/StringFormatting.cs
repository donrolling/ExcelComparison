using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Extensions {
	public static class StringFormatting {

		public static string GetPrimaryKeyName<T>() where T : class {
			return string.Concat(UnCamelCaseToUnderscores(typeof(T).ToString()).ToLower(), "_id");
		}

		public static string MustNotBeEmpty(this string value, string nameOfVariable) {
			if (string.IsNullOrEmpty(value)) {
				throw new Exception($"Value Not Provided: { nameOfVariable } was null or empty. This is not allowed.");
			}
			return value;
		}

		public static int NthIndexOf(this string target, string value, int n) {
			var m = Regex.Match(target, "((" + value + ").*?){" + n + "}");
			if (m.Success)
				return m.Groups[2].Captures[n - 1].Index;
			else
				return -1;
		}

		public static string SplitCamelCase(this string str) {
			return Regex.Replace(
				Regex.Replace(
					str,
					@"(\P{Ll})(\P{Ll}\p{Ll})",
					"$1 $2"
				),
				@"(\p{Ll})(\P{Ll})",
				"$1 $2"
			);
		}

		public static string ToCsv(this IEnumerable<string> list, bool addSpace = true) {
			if (addSpace) {
				return list.Aggregate(string.Empty, (a, next) => $"{ a },{ next }").Trim(',');
			} else {
				return list.Aggregate(string.Empty, (a, next) => $"{ a }, { next }").Trim(',');
			}
		}

		public static string UnCamelCase(this string value) {
			return uncamelCase(value, " ");
		}

		public static string UnCamelCaseToUnderscores(this string value) {
			return uncamelCase(value, "_");
		}

		private static string uncamelCase(string value, string replacementString) {
			if (string.IsNullOrEmpty(value)) {
				return value;
			}

			var sb = new StringBuilder();
			var previousIsSpace = false;
			foreach (char c in value) {
				if (char.IsLower(c) || c == ' ') {
					if (c == ' ') {
						previousIsSpace = true;
					}
					sb.Append(c);
				} else {
					if (previousIsSpace) {
						sb.Append(c);
					} else {
						sb.Append(replacementString + c);
					}
					previousIsSpace = false;
				}
			}
			return sb.ToString().Trim();
		}
	}
}