using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Randomization {
	public static class RandomStrings {
		public static Random Random;
		
		static RandomStrings() {
			Random = RandomProvider.GetThreadRandom();
		}

		public static string GetRandomName(int length = 5) {
			return string.Concat("", randomize(length));
		}

		public static string GetRandomName(string baseName, int length = 5) {
			return string.Concat(baseName, randomize(length));
		}

		public static string GetRandomPassword(int length = 12) {
			return Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);
		}

		public static string GetRandomText(int length, bool includeSpaces = true) {
			if (length < 1) {
				return string.Empty;
			}
			var chars = includeSpaces ?
							"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 "
							:
							"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var result = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
			if (string.IsNullOrEmpty(result)) { //prevent empty results
				return GetRandomText(length, includeSpaces);
			}
			return result;
		}

		private static string randomize(int length) {
			const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
			var chars = new char[length];
			for (int i = 0; i < length; i++) {
				chars[i] = allowedChars[Random.Next(0, allowedChars.Length)];
			}
			return new string(chars);
		}
	}
}
