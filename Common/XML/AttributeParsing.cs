using Common.Conversion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Common.XML {
	public class AttributeParsing {

		public static T ParseXElementAttribute<T>(XElement xElement, string attributeName) where T : struct {
			var value = xElement.Attribute(attributeName).Value;
			return TypeCoercion.ConvertFromString<T>(value);
		}

		//public static T ParseXNodeAttribute<T>(XElement xElement, string attributeName) where T : struct {
		//	var value = xElement.Attribute(attributeName).Value;
		//	return TypeCoercion.ConvertFromString<T>(value);
		//}
	}
}