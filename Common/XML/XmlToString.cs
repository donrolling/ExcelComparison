using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Common.XML {
	public class XmlToString {

		public static string ToString(IXmlSerializable items, bool writeStartAndEndDocument = false) {
			if (!writeStartAndEndDocument) {
				using (var stringWriter = new StringWriter())
				using (var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings())) {
					items.WriteXml(xmlTextWriter);
					xmlTextWriter.Flush();
					return stringWriter.GetStringBuilder().ToString();
				}
			} else {
				using (var stringWriter = new StringWriter())
				using (var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Auto })) {
					xmlTextWriter.WriteStartDocument();
					items.WriteXml(xmlTextWriter);
					xmlTextWriter.WriteEndDocument();
					xmlTextWriter.Flush();
					return stringWriter.GetStringBuilder().ToString();
				}
			}
		}

		public static string ToString(XNode item) {
			using (var stringWriter = new StringWriter())
			using (var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment })) {
				item.WriteTo(xmlTextWriter);
				xmlTextWriter.Flush();
				return stringWriter.GetStringBuilder().ToString();
			}
		}
	}
}