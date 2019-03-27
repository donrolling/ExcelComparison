using Common.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Business.Common
{
	public class FileUtility
	{
		public static string CleanFilename(string filename, string replaceWith = "")
		{
			var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(filename, replaceWith);
		}

		public static string GetBasePath<T>() where T : class
		{
			var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
			var pos = basePath.IndexOf("\\bin");
			if (pos >= 0) {
				basePath = basePath.Substring(0, pos);
			}
			return basePath;
		}

		public static string GetFullPath<T>(string subDirectory) where T : class
		{
			var path = string.Concat(GetBasePath<T>(), subDirectory);
			return path;
		}

		public static string GetFullPath_FromRelativePath<T>(string filename, string subDirectory) where T : class
		{
			var path = Path.Combine(GetBasePath<T>(), subDirectory, filename);
			return path;
		}

		public static FileStream OpenRead<T>(string filename, string subDirectory) where T : class
		{
			if (subDirectory.Contains(":")) {
				var path = ProperPathCombine.Combine(new List<string> { subDirectory, filename });
				return File.OpenRead(path);
			} else {
				var basePath = GetBasePath<T>();
				var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
				return File.OpenRead(path);
			}
		}

		/// <summary>
		/// Reads a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <returns></returns>
		public static string ReadTextFile<T>(string filename, string subDirectory) where T : class
		{
			var path = Path.Combine(GetBasePath<T>(), subDirectory, filename);
			return ReadTextFile(path);
		}

		public static string ReadTextFile(string filePath)
		{
			if (filePath.Contains(":\\")) {
				return File.ReadAllText(filePath);
			}
			var path = string.Concat(GetBasePath<FileUtility>(), filePath);
			return File.ReadAllText(path);
		}

		public static void WriteFile<T>(string filename, string subDirectory, ExcelPackage excelPackage) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(FileUtility.CleanFilename(filename, "_"), subDirectory);
			var data = excelPackage.GetAsByteArray();
			var dirName = Path.GetDirectoryName(path);
			Directory.CreateDirectory(dirName);
			File.WriteAllBytes(path, data);
		}

		public static void WriteFile(string path, string contents)
		{
			if (!Directory.Exists(path)) {
				var dirName = Path.GetDirectoryName(path);
				Directory.CreateDirectory(dirName);
			}
			File.WriteAllText(path, contents);
		}

		public static void WriteFile<T>(string filename, string subDirectory, byte[] bytes) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(filename, subDirectory);
			var pathOnly = Path.GetDirectoryName(path);
			Directory.CreateDirectory(pathOnly);
			File.WriteAllBytes(path, bytes);
		}

		public static void WriteFile<T>(string filename, string subDirectory, string contents) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(filename, subDirectory);
			if (!Directory.Exists(path)) {
				var dirName = Path.GetDirectoryName(path);
				Directory.CreateDirectory(dirName);
			}
			File.WriteAllText(path, contents);
		}
	}
}