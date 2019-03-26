using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.DataSet {
	public class DataSetToString {

		public static string ToString(System.Data.DataSet dataSet) {
			var sw = new StringWriter();
			dataSet.WriteXml(sw);
			return sw.ToString();
		}
	}
}