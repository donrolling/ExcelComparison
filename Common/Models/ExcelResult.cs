using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models {
	public class ExcelResult : IDisposable {
		public string Directory { get; set; }
		public ExcelPackage ExcelPackage { get; set; }
		public string Filename { get; set; }

		public void Dispose() {
			this.ExcelPackage.Dispose();
		}
	}
}