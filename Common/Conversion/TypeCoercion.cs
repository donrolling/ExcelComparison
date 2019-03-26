using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.Conversion {
	public class TypeCoercion {

		public static T ConvertFromObject<T>(object input) where T : struct {
			try {
				var converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null) {
					return (T)converter.ConvertTo(input, typeof(T));
				}
				return default(T);
			} catch (NotSupportedException) {
				return default(T);
			}
		}

		public static T ConvertFromString<T>(string input) where T : struct {
			try {
				var converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null) {
					return (T)converter.ConvertFromString(input);
				}
				return default(T);
			} catch (NotSupportedException) {
				return default(T);
			}
		}

		public static R ConvertFromType<T, R>(T input) where T : struct where R : struct {
			try {
				var converter = TypeDescriptor.GetConverter(typeof(R));
				if (converter != null) {
					return (R)converter.ConvertTo(input, typeof(R));
				}
				return default(R);
			} catch (NotSupportedException) {
				return default(R);
			}
		}
	}
}