using Common.Randomization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tests.Utilities {
	public static class TestUtilities {
		public static Random Random;

		static TestUtilities() {
			Random = RandomProvider.GetThreadRandom();
		}

		public static bool ArePropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class {
			if (self == null && to == null) {
				return true;
			}
			if (self == null || to == null) {
				return false;
			}
			var type = typeof(T);
			var ignoreList = new List<string>(ignore);
			var unequalProperties =
				from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				where !ignoreList.Contains(pi.Name)
				let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
				let toValue = type.GetProperty(pi.Name).GetValue(to, null)
				where
					selfValue != toValue
					&& (selfValue == null || !selfValue.Equals(toValue))
				select pi;

			if (!unequalProperties.Any()) {
				return true;
			}
			//eliminate DateTime properties, which don't compare the same, from the equation
			var dateProps = unequalProperties.Where(a => a.PropertyType.Name == typeof(DateTime).Name);
			if (!dateProps.Any()) {
				return false;
			}
			var count = 0;
			foreach (var pi in dateProps) {
				var selfValue = type.GetProperty(pi.Name).GetValue(self, null);
				var toValue = type.GetProperty(pi.Name).GetValue(to, null);
				if (DatesAreEqual((DateTime)selfValue, (DateTime)toValue)) {
					count++;
				}
			}
			return count == unequalProperties.Count();
		}

		public static string FileName_WithDateTime(string baseFileName, string fileExtension) {
			var dateTimeString = DateTime.Now.ToString("MM/dd/yy HH:mm:ss").Replace("/", ".").Replace(" ", "_").Replace(":", ".");
			return string.Concat(baseFileName, "_", dateTimeString, fileExtension);
		}

		public static string GetBasePath<T>() where T : class {
			var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
			var pos = basePath.IndexOf("\\bin");
			if (pos >= 0) {
				basePath = basePath.Substring(0, pos);
			}
			return basePath;
		}

		public static string GetFullPath_FromRelativePath<T>(string filename, string subDirectory) where T : class {
			var path = Path.Combine(GetBasePath<T>(), subDirectory, filename);
			return path;
		}

		public static string GetRandomEmail() {
			var limit = Random.Next(6, 10);
			var front = new StringBuilder(limit);
			var back = new StringBuilder(limit);
			for (int i = 0; i < limit; i++) {
				var f = getRandomAlphaNumericChar();
				front.Append(f);
				var b = getRandomAlphaNumericChar();
				back.Append(b);
			}
			var ptrn = "{0}@{1}.com";
			return string.Format(ptrn, front, back);
		}

		public static string GetRandomEmails(int num) {
			var result = new StringBuilder();
			for (int i = 0; i < num; i++) {
				result.Append(GetRandomEmail());
				result.Append(",");
			}
			return result.ToString().Trim(',');
		}

		public static long GetRandomLong() {
			var max = long.MaxValue;
			var min = 0;//didn't want a negative
			var buf = new byte[8];
			Random.NextBytes(buf);
			var longRand = BitConverter.ToInt64(buf, 0);
			return (Math.Abs(longRand % (max - min)) + min);
		}		

		public static long RandomLong(long min, long max, Random rand) {
			var buf = new byte[8];
			rand.NextBytes(buf);
			long longRand = BitConverter.ToInt64(buf, 0);
			return (Math.Abs(longRand % (max - min)) + min);
		}

		/// <summary>
		/// Reads a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <returns></returns>
		public static string ReadTextFile<T>(string filename, string subDirectory) where T : class {
			var basePath = GetBasePath<T>();
			var path = concatPath(basePath, filename, subDirectory);
			var data = File.ReadAllText(path);
			return data;
		}

		public static List<string> ReadTextFile_ToListOfString<T>(string filename, string subDirectory) where T : class {
			var basePath = GetBasePath<T>();
			var path = concatPath(basePath, filename, subDirectory);
			var result = new List<string>();
			var line = string.Empty;
			using (var file = new System.IO.StreamReader(path)) {
				while ((line = file.ReadLine()) != null) {
					if (!string.IsNullOrEmpty(line)) {
						result.Add(line);
					}
				}
			}
			return result;
		}

		public static List<string> WhichPropertiesAreNotEqual<T>(this T self, T to, params string[] ignore) where T : class {
			var type = typeof(T);
			var ignoreList = new List<string>(ignore);
			var unequalProperties =
				from pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				where !ignoreList.Contains(pi.Name)
				let selfValue = type.GetProperty(pi.Name).GetValue(self, null)
				let toValue = type.GetProperty(pi.Name).GetValue(to, null)
				where selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue))
				select pi.Name;
			return unequalProperties.ToList();
		}

		/// <summary>
		/// Writes a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <param name="contents">The text of the file that you'd like to write.</param>
		/// <returns></returns>
		public static void WriteFile<T>(string filename, string subDirectory, string contents) where T : class {
			var path = GetFullPath_FromRelativePath<T>(filename, subDirectory);
			File.WriteAllText(path, contents);
		}

		private static string concatPath(string basePath, string filename, string subDirectory) {
			//combine path wasn't working right here, so needsSlash1 and needsSlash2
			var needsSlash1 = !(basePath.EndsWith("\\") || subDirectory.StartsWith("\\"));
			var needsSlash2 = !(subDirectory.EndsWith("\\") || filename.StartsWith("\\"));
			var path = string.Concat(basePath, needsSlash1 ? "\\" : "", subDirectory, needsSlash2 ? "\\" : "", filename);
			return path;
		}

		private static bool DatesAreEqual(DateTime d1, DateTime d2) {
			if (
				d1.Minute == d2.Minute
				&& d1.Day == d2.Day
				&& d1.Second == d2.Second
				&& d1.Year == d2.Year
			) {
				return true;
			}
			return false;
		}

		private static char getRandomAlphaNumericChar() {
			return Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
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