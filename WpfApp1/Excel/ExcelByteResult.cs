using System;

namespace Common.Models {
	[Serializable]
	public class ExcelByteResult {
		public string Directory { get; set; }
		public byte[] ExcelBytes { get; set; }
		public string Filename { get; set; }
	}
}