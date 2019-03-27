using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models {
	public class ExcelTest_CoordinateInfo {
		/// <summary>
		/// The starting column for comparison
		/// </summary>
		public int ColumnOffset { get; set; } = 1;
		/// <summary>
		/// The number of columns to compare
		/// </summary>
		public int Columns { get; set; }
		/// <summary>
		/// The starting row for comparison
		/// </summary>
		public int RowOffset { get; set; } = 1;
		/// <summary>
		/// The number of rows to compare
		/// </summary>
		public int Rows { get; set; }
	}
}