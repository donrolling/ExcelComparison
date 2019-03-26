using Common.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;

namespace Common.Interfaces {
	public interface IExcelBaseService {

		void AlignRight(ExcelRange excelRange);

		void AutoFit_All_Columns(ExcelWorksheet ws);

		void AutoFitColumns(ExcelRange excelRange);

		string ColumnLetter_FromColumnNumber(int columnNumber);

		int ColumnNumber_FromColumnLetter(string columnName);

		ExcelResult CopyExcelFile(ExcelResult actual, string errorFilename);

		void Format_RedBackground_BlackText(ExcelRange excelRange, string message);

		void FormatNumber(ExcelRange excelRange, int precision);

		void FormatNumber(ExcelRange excelRange, int precision, decimal value);

		void FormatPercentage(ExcelRange excelRange, int precision);

		void FormatPercentage(ExcelRange excelRange, int precision, decimal value, bool divideBy100 = true);

		void Merge(ExcelRange range);

		void MergeAndCenter(ExcelRange range);

		void MergeAndCenter(ExcelRange range, string value);

		void MergeAndCenterVerticallyAndHorizontally(ExcelRange range, bool wrap = false);

		ExcelResult ReadFile(string filename, string subDirectory);

		void Save(ExcelResult excelResult);

		void Save(string filename, string directory, byte[] data);

		void Save(string filename, string directory, FileStreamResult data);

		void SetHeader(ExcelRange cell, string value);

		void SetHeaderStyles(ExcelRange excelRange);

		MemoryStream ToMemoryStream(ExcelResult excelResult);

		MemoryStream ToMemoryStream(ExcelPackage excelPackage);
	}
}