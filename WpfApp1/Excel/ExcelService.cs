using Business.Common;
using Common.IO;
using Common.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;

namespace Business.Services.Excel
{
	public class ExcelService
	{
		public ExcelService()
		{
		}

		public void AlignRight(ExcelRange excelRange)
		{
			excelRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
		}

		public void AutoFit_All_Columns(ExcelWorksheet excelWorksheet)
		{
			this.AutoFitColumns(excelWorksheet.Cells[excelWorksheet.Dimension.Address]);
		}

		public void AutoFitColumns(ExcelRange excelRange)
		{
			excelRange.AutoFitColumns();
		}

		public string ColumnLetter_FromColumnNumber(int columnNumber)
		{
			var dividend = columnNumber;
			var columnName = string.Empty;
			var modulo = 0;
			while (dividend > 0) {
				modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
				dividend = (int)((dividend - modulo) / 26);
			}
			return columnName;
		}

		public int ColumnNumber_FromColumnLetter(string columnName)
		{
			columnName = columnName.ToUpperInvariant();
			int sum = 0;
			for (int i = 0; i < columnName.Length; i++) {
				sum *= 26;
				sum += (columnName[i] - 'A' + 1);
			}
			return sum;
		}

		public ExcelResult CopyExcelFile(ExcelResult fileToCopy, string newFileName)
		{
			//gotta read it again, because the stream will be closed
			using (var file = this.ReadFile(fileToCopy.Filename, fileToCopy.Directory)) {
				var result = new ExcelResult
				{
					Filename = newFileName,
					Directory = fileToCopy.Directory,
					ExcelPackage = file.ExcelPackage
				};
				this.Save(result);
				return result;
			}
		}

		public void Format_Background_Text(ExcelRange excelRange, ExcelFillStyle excelFillStyle, ExcelHorizontalAlignment excelHorizontalAlignment, Color color, bool boldText)
		{
			excelRange.Style.Fill.PatternType = excelFillStyle;
			excelRange.Style.Fill.BackgroundColor.SetColor(color);
			excelRange.Style.Font.Bold = true;
			excelRange.Style.HorizontalAlignment = excelHorizontalAlignment;
		}

		public void Format_RedBackground_BlackText(ExcelRange excelRange, string message)
		{
			excelRange.Value = message;
			excelRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
			var colFromHex = System.Drawing.ColorTranslator.FromHtml("#FF0000");
			excelRange.Style.Fill.BackgroundColor.SetColor(colFromHex);
		}

		public void FormatNumber(ExcelRange excelRange, int precision, decimal value)
		{
			excelRange.Value = value;
			this.FormatNumber(excelRange, precision);
		}

		public void FormatNumber(ExcelRange excelRange, int precision)
		{
			var format = this.getNumberFormat_WithPrecision(precision);
			excelRange.Style.Numberformat.Format = format;
		}

		public void FormatPercentage(ExcelRange excelRange, int precision, decimal value, bool divideBy100 = true)
		{
			excelRange.Value = value == 0 ? 0 : divideBy100 ? value / 100 : value;
			this.FormatPercentage(excelRange, precision);
		}

		public void FormatPercentage(ExcelRange excelRange, int precision)
		{
			excelRange.Style.Numberformat.Format = this.getPercentageFormat_WithPrecision(precision);
		}

		public ExcelPackage FromMemoryStream(MemoryStream ms)
		{
			var excelPackage = new ExcelPackage(ms);
			return excelPackage;
		}

		public void Merge(ExcelRange range)
		{
			range.Merge = true;
		}

		public void MergeAndCenter(ExcelRange range, string value)
		{
			this.MergeAndCenter(range);
			range.Value = value;
		}

		public void MergeAndCenter(ExcelRange range)
		{
			range.Merge = true;
			range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		}

		public void MergeAndCenterVerticallyAndHorizontally(ExcelRange range, bool wrap = false)
		{
			range.Merge = true;
			if (wrap) {
				range.Style.WrapText = true;
			}
			range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
		}

		public ExcelResult ReadFile(string filename, string directory)
		{
			using (var filestream = FileUtility.OpenRead<ExcelService>(cleanseFilename(filename), directory)) {
				return new ExcelResult
				{
					Filename = filename,
					Directory = directory,
					ExcelPackage = new ExcelPackage(filestream)
				};
			}
		}
		
		public ExcelResult ReadFile(string path)
		{
			var filename = Path.GetFileName(path);
			var directory = Path.GetDirectoryName(path);
			return ReadFile(filename, directory);
		}

		public void Save(string filename, string directory, byte[] data)
		{
			FileUtility.WriteFile<ExcelService>(filename, directory, data);
		}

		public void Save(ExcelResult excelResult)
		{
			if (string.IsNullOrEmpty(excelResult.Directory)) {
				throw new Exception("excelResult.Directory cannot be null or empty.");
			}
			var filename = cleanseFilename(excelResult.Filename);
			FileUtility.WriteFile<ExcelService>(filename, excelResult.Directory, excelResult.ExcelPackage.GetAsByteArray());
		}

		public void SaveExcel(string filename, string directory, ExcelPackage result)
		{
			FileUtility.WriteFile<ExcelService>(this.cleanseFilename(filename), directory, result);
		}

		public void SetHeader(ExcelRange cell, string value)
		{
			cell.Value = value;
			SetHeaderStyles(cell);
		}

		public void SetHeaderStyles(ExcelRange excelRange)
		{
			excelRange.Style.Font.Bold = true;
			excelRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
		}

		public MemoryStream ToMemoryStream(ExcelResult excelResult)
		{
			return this.ToMemoryStream(excelResult.ExcelPackage);
		}

		public MemoryStream ToMemoryStream(ExcelPackage excelPackage)
		{
			return new MemoryStream(excelPackage.GetAsByteArray());
		}

		private string cleanseFilename(string filename)
		{
			var fn = filename.Replace(".xlsx", "").Replace(".xls", "");
			return FileUtility.CleanFilename(fn, "_") + ".xlsx";
		}

		private string getNumberFormat_WithPrecision(int precision)
		{
			var baseFormat = "#,##0_);[Red](#,##0)";
			if (precision == 0) { return baseFormat; }
			var zeros = new String('0', precision);
			return baseFormat.Replace("#0", $"#0.{ zeros }");
		}

		private string getPercentageFormat_WithPrecision(int precision)
		{
			var baseFormat = "#,##0%_);[Red](#,##0%)";
			if (precision == 0) { return baseFormat; }
			var zeros = new String('0', precision);
			return baseFormat.Replace("#0", $"#0.{ zeros }");
		}
	}
}