using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Common.XML {
	public static class XMLSerialize {

		public static string Serialize(this object x) {
			var xml = string.Empty;
			var xs = new XmlSerializer(x.GetType());
			using (var sww = new StringWriter()) {
				using (var writer = XmlWriter.Create(sww)) {
					xs.Serialize(writer, x);
					xml = sww.ToString();
				}
			}
			return xml;
		}
	}
}