using Common.Conversion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Common.Parsing {
	public class DataRowParsing {

		public static T ParseDataRowProperty<T>(DataRow row, string propertyName) where T : struct {
			var value = row[propertyName];
			return TypeCoercion.ConvertFromObject<T>(value);
		}
	}
}