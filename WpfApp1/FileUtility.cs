using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.IO
{
	public class FileUtility
	{
		public static string RootBinPath {
			get {
				if (string.IsNullOrEmpty(_rootBinPath)) {
					_rootBinPath = Path.GetDirectoryName(typeof(FileUtility).Assembly.GetName().CodeBase);
				}
				return _rootBinPath;
			}
		}

		private const string physicalBinDir = "\\bin\\";
		private const char physicalPathDelimiter = '\\';
		private const string webBinDir = "/bin/";
		private const char webPathDelimiter = '/';
		private static string _rootBinPath = "";

		public static string CleanFilename(string filename, string replaceWith = "")
		{
			var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(filename, replaceWith);
		}

		public static string GetBasePath<T>(bool fileResidesInBin) where T : class
		{
			var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
			if (fileResidesInBin) {
				return basePath;
			}
			var pos = basePath.IndexOf("\\bin");
			if (pos >= 0) {
				basePath = basePath.Substring(0, pos);
			}
			return basePath;
		}

		public static Tuple<string, string> GetFullPath_FromRelativePath<T>(string filename, string subDirectory = "") where T : class
		{
			var basePath = GetBasePath<T>(false);
			var path = string.IsNullOrEmpty(subDirectory) ? basePath : ProperPathCombine.Combine(new List<string> { basePath, subDirectory });
			var filePath = ProperPathCombine.Combine(new List<string> { path, filename });
			return Tuple.Create<string, string>(path, filePath);
		}

		public static FileStream OpenRead<T>(string filename, string subDirectory, bool fileResidesInBin = false) where T : class
		{
			var basePath = GetBasePath<T>(fileResidesInBin);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
			return File.OpenRead(path);
		}

		public static IEnumerable<string> ReadLines<T>(string filename, string subDirectory) where T : class
		{
			var basePath = GetBasePath<T>(false);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
			var result = new List<string>();
			var counter = 0;
			var line = string.Empty;
			using (var file = new System.IO.StreamReader(path)) {
				while ((line = file.ReadLine()) != null) {
					if (!string.IsNullOrEmpty(line)) {
						result.Add(line);
					}
					counter++;
				}
			}
			return result;
		}

		/// <summary>
		/// Reads a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <returns></returns>
		public static string ReadTextFile<T>(string filename, string subDirectory, bool fileResidesInBin = false) where T : class
		{
			var basePath = GetBasePath<T>(fileResidesInBin);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename }); ;
			var data = File.ReadAllText(path);
			return data;
		}

		public static string ReadTextFile(string filename, string baseServerPath, string subDirectory)
		{
			var path = ProperPathCombine.Combine(new List<string> { baseServerPath, subDirectory, filename });
			var data = File.ReadAllText(path);
			return data;
		}

		/// <summary>
		/// Don't specify bin, we'll take care of that for you.
		/// </summary>
		/// <param name="fileProvider"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadTextFileFromBin(string path)
		{
			//protect us from ourselves
			var tidyPath = path
							.TrimStart(webPathDelimiter)
							.TrimStart(physicalPathDelimiter)
							.Replace(webBinDir, string.Empty)
							.Replace(physicalBinDir, string.Empty);
			//if this is on web we want it to work the same in a unit test
			var basePathSplit = RootBinPath.Split(physicalBinDir);
			var partialPath = basePathSplit.Length > 1
				? $"{ physicalBinDir }{ basePathSplit[1] }"
				: string.Empty;
			var safePath = $"{ partialPath }\\{ tidyPath }".Replace(physicalPathDelimiter, webPathDelimiter);
			//protecting us from web vs file system reading issues
			if (fileProvider.GetFileInfo(safePath).Exists) {
				return readFileViaProvider(fileProvider, safePath);
			} else {
				safePath = $"{ tidyPath }".Replace(physicalPathDelimiter, webPathDelimiter);
				return readFileViaProvider(fileProvider, safePath);
			}
		}

		public static void WriteFile<T>(string filename, string subDirectory, byte[] bytes) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(filename, subDirectory);
			Directory.CreateDirectory(path.Item1);
			File.WriteAllBytes(path.Item2, bytes);
		}

		public static void WriteFile<T>(string filename, string subDirectory, ExcelPackage excelPackage) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(FileUtility.CleanFilename(filename, "_"), subDirectory);
			var data = excelPackage.GetAsByteArray();
			Directory.CreateDirectory(path.Item1);
			File.WriteAllBytes(path.Item2, data);
		}

		public static void WriteFile<T>(string filename, string subDirectory, string contents) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(FileUtility.CleanFilename(filename, "_"), subDirectory);
			Directory.CreateDirectory(path.Item1);
			File.WriteAllText(path.Item2, contents);
		}

		public static void WriteFile<T>(string filename, string subDirectory, Stream contents) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(FileUtility.CleanFilename(filename, "_"), subDirectory);
			Directory.CreateDirectory(path.Item1);
			var output = new FileStream(path.Item2, FileMode.Create);
			contents.CopyTo(output);
		}

		private static string readFileViaProvider(string safePath)
		{
			using (var stream = .GetFileInfo(safePath).CreateReadStream())
			using (var reader = new StreamReader(stream, Encoding.UTF8)) {
				var sb = new StringBuilder();
				string line;
				while ((line = reader.ReadLine()) != null) {
					sb.AppendLine(line);
				}
				return sb.ToString();
			}
		}
	}
}