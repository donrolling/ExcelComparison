using Business.Enums;
using Business.Interfaces;
using Common.BaseClasses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Business.Services.Excel {
	public class ObjectToExcelUtility {
		public ExcelService ExcelService { get; }

		public ObjectToExcelUtility() {
			this.ExcelService = new ExcelService();
		}

		public void Object_To_Excel(object data, ExcelPackage package, string worksheetName, ObjectToExcel_PropertyListOptions propertyListOptions) {
			if (data == null) {
				throw new Exception("Object_To_ExcelPackage: Null or empty input table!\n");
			}
			var properties = getProperties(data);
			var ws = package.Workbook.Worksheets.Add(worksheetName);
			var json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
			var range = ws.Cells[1, 1];
			range.Value = json;
			range.Style.WrapText = true;
			range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
			this.ExcelService.AutoFit_All_Columns(ws);
			ws.Column(1).Width = 200;
		}

		public void ObjectList_To_Excel(IEnumerable<object> data, ExcelPackage package, string worksheetName, ObjectToExcel_PropertyListOptions propertyListOptions) {
			if (data == null || !data.Any()) {
				throw new Exception("Object_To_ExcelPackage: Null or empty input table!\n");
			}
			var properties = getProperties(data.First());
			var ws = package.Workbook.Worksheets.Add(worksheetName);
			setHeaders(properties, ws, propertyListOptions);
			setData(properties, data.ToList(), ws, propertyListOptions);
			this.ExcelService.AutoFit_All_Columns(ws);
		}

		public ExcelPackage ObjectList_To_ExcelPackage(IEnumerable<object> data, ObjectToExcel_PropertyListOptions propertyListOptions) {
			var package = new ExcelPackage();
			return this.ObjectList_To_ExcelPackage(data, package, "Worksheet", propertyListOptions);
		}

		public ExcelPackage ObjectList_To_ExcelPackage(IEnumerable<object> data, string worksheetName, ObjectToExcel_PropertyListOptions propertyListOptions) {
			var package = new ExcelPackage();
			return this.ObjectList_To_ExcelPackage(data, package, worksheetName, propertyListOptions);
		}

		public ExcelPackage ObjectList_To_ExcelPackage(IEnumerable<object> data, ExcelPackage package, string worksheetName, ObjectToExcel_PropertyListOptions propertyListOptions) {
			if (data == null || !data.Any()) {
				throw new Exception("Object_To_ExcelPackage: Null or empty input table!\n");
			}
			var properties = getProperties(data.First());
			var ws = package.Workbook.Worksheets.Add(worksheetName);
			setHeaders(properties, ws, propertyListOptions);
			setData(properties, data.ToList(), ws, propertyListOptions);
			this.ExcelService.AutoFit_All_Columns(ws);
			return package;
		}

		public void Save_Object_To_Excel(string filename, string outputDirectory, IEnumerable<object> data, ObjectToExcel_PropertyListOptions propertyListOptions) {
			var result = this.ObjectList_To_ExcelPackage(data, propertyListOptions);
			this.ExcelService.SaveExcel(filename, outputDirectory, result);
		}

		private (int Row, int Column) getDataRange(ExcelWorksheet ws, int itemIndex, int propertyIndex, ObjectToExcel_PropertyListOptions propertyListOptions, (int Row, int Column) pickupRange) {
			var column = 0;
			var row = 0;
			switch (propertyListOptions) {
				case ObjectToExcel_PropertyListOptions.HorizontalPropertyNames:
					if (pickupRange.Row == 0) {
						row = itemIndex + 2;
					} else {
						row = pickupRange.Row;
					}
					if (pickupRange.Column == 0) {
						column = itemIndex + 1;
					} else {
						column = pickupRange.Column + 1;
					}
					break;
				case ObjectToExcel_PropertyListOptions.VerticalPropertyNames:
					if (pickupRange.Row == 0) {
						row = propertyIndex + 1;
					} else {
						row = pickupRange.Row + 1;
					}
					if (pickupRange.Column == 0) {
						column = itemIndex + 2;
					} else {
						column = pickupRange.Column;
					}
					break;
				default:
					throw new Exception("Case not matched.");
			}
			return (row, column);
		}

		private ExcelRange getFullHeadingRange(ExcelWorksheet ws, int count, ObjectToExcel_PropertyListOptions propertyListOptions) {
			switch (propertyListOptions) {
				case ObjectToExcel_PropertyListOptions.HorizontalPropertyNames:
					return ws.Cells[1, 1, 1, count];
				case ObjectToExcel_PropertyListOptions.VerticalPropertyNames:
					return ws.Cells[1, 1, count, 1];
				default:
					throw new Exception("Case not matched.");
			}
		}

		private ExcelRange getHeadingRange(ExcelWorksheet ws, int itemIndex, ObjectToExcel_PropertyListOptions propertyListOptions) {
			var column = 0;
			var row = 0;
			switch (propertyListOptions) {
				case ObjectToExcel_PropertyListOptions.HorizontalPropertyNames:
					row = 1;
					column = itemIndex + 1;
					break;
				case ObjectToExcel_PropertyListOptions.VerticalPropertyNames:
					column = 1;
					row = itemIndex + 1;
					break;
				default:
					throw new Exception("Case not matched.");
			}
			return ws.Cells[row, column];
		}

		private List<PropertyInfo> getProperties(object obj) {
			return obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
		}

		private (int Row, int Column) setData(List<PropertyInfo> properties, object data, ExcelWorksheet ws, ObjectToExcel_PropertyListOptions propertyListOptions, int pickupRow = 0, int pickupColumn = 0) {
			var itemIndex = 0;
			(int Row, int Column) pickupRange = (pickupRow, pickupColumn);
			for (int propertyIndex = 0; propertyIndex < properties.Count(); propertyIndex++) {
				pickupRange = setField(properties, data, ws, propertyListOptions, itemIndex, propertyIndex, pickupRange);
			}
			return pickupRange;
		}

		private void setData(List<PropertyInfo> properties, List<object> data, ExcelWorksheet ws, ObjectToExcel_PropertyListOptions propertyListOptions) {
			(int Row, int Column) pickupRange = (0, 0);
			for (int itemIndex = 0; itemIndex < data.Count(); itemIndex++) {
				for (int propertyIndex = 0; propertyIndex < properties.Count(); propertyIndex++) {
					pickupRange = setField(properties, data[itemIndex], ws, propertyListOptions, itemIndex, propertyIndex, pickupRange);
				}
			}
		}

		private (int Row, int Column) setField(List<PropertyInfo> properties, object data, ExcelWorksheet ws, ObjectToExcel_PropertyListOptions propertyListOptions, int itemIndex, int propertyIndex, (int Row, int Column) pickupRange) {
			var value = properties[propertyIndex].GetValue(data, null);
			var isValueTypeOrString = true;
			Type valueType = null;
			var valueTypeName = "";
			if (value != null) {
				valueType = value.GetType();
				valueTypeName = valueType.Name;
				if (valueTypeName == "String") {
					isValueTypeOrString = true;
				} else {
					isValueTypeOrString = valueType.IsValueType;
				}
			}
			if (isValueTypeOrString) {
				var range = this.getDataRange(ws, itemIndex, propertyIndex, propertyListOptions, pickupRange);
				var excelRange = ws.Cells[range.Row, range.Column];
				excelRange.Value = value;
				this.ExcelService.AlignRight(excelRange);
				return range;
			} else {
				var newProperties = getProperties(value);
				var range = this.getDataRange(ws, itemIndex, propertyIndex, propertyListOptions, pickupRange);
				var excelRange = ws.Cells[range.Row, range.Column];
				excelRange.Value = valueTypeName;
				this.ExcelService.AlignRight(excelRange);
				switch (propertyListOptions) {
					case ObjectToExcel_PropertyListOptions.HorizontalPropertyNames:
						this.setData(newProperties, value, ws, ObjectToExcel_PropertyListOptions.VerticalPropertyNames, range.Row + 1, range.Column);
						return (range.Row, range.Column);
					case ObjectToExcel_PropertyListOptions.VerticalPropertyNames:
						this.setData(newProperties, value, ws, ObjectToExcel_PropertyListOptions.HorizontalPropertyNames, range.Row, range.Column + 1);
						return (range.Row, range.Column);
					default:
						throw new Exception("Case not matched.");
				}
			}
		}

		private void setHeaders(List<PropertyInfo> properties, ExcelWorksheet ws, ObjectToExcel_PropertyListOptions propertyListOptions) {
			var count = properties.Count();
			for (int i = 0; i < count; i++) {
				var property = properties[i];
				var range = this.getHeadingRange(ws, i, propertyListOptions);
				range.Value = property.Name;
			}
			var headerRange = this.getFullHeadingRange(ws, count, propertyListOptions);
			switch (propertyListOptions) {
				case ObjectToExcel_PropertyListOptions.HorizontalPropertyNames:
					this.ExcelService.Format_Background_Text(headerRange, ExcelFillStyle.Solid, ExcelHorizontalAlignment.Center, Color.LightGray, true);
					break;
				case ObjectToExcel_PropertyListOptions.VerticalPropertyNames:
					this.ExcelService.Format_Background_Text(headerRange, ExcelFillStyle.Solid, ExcelHorizontalAlignment.Left, Color.LightGray, true);
					break;
				default:
					break;
			}
		}
	}
}