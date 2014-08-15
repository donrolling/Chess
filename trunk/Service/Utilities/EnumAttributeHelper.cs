using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Service.Utilities {
	public static class EnumAttributeHelper {
		public static string GetDesciption(object value) {
			string name = value.ToString();
			var enumType = value.GetType();
			var field = enumType.GetField(name);
			object[] attrs = field.GetCustomAttributes(true);
			if (attrs.Length > 0) {
				DisplayAttribute da = (DisplayAttribute)attrs[0];
				return da.Description;
			}
			return name.UnCamelCase();
		}
		public static string GetName(object value) {
			string name = value.ToString();
			var enumType = value.GetType();
			var field = enumType.GetField(name);
			object[] attrs = field.GetCustomAttributes(true);
			if (attrs.Length > 0) {
				DisplayAttribute da = (DisplayAttribute)attrs[0];
				return da.Name;
			}
			return name.UnCamelCase();
		}
		public static EnumNameDescriptionValue GetEnumNameAndDescription(object value) {
			string name = value.ToString();
			var enumType = value.GetType();
			var field = enumType.GetField(name);
			object[] attrs = field.GetCustomAttributes(true);
			if (attrs.Length > 0) {
				DisplayAttribute da = (DisplayAttribute)attrs[0];
				return new EnumNameDescriptionValue() {
					Name = da.Name,
					Description = da.Description
				};
			}
			return new EnumNameDescriptionValue() {
				Name = name.UnCamelCase(),
				Description = name.UnCamelCase()
			};
		}
		public static List<EnumNameDescriptionValue> GetListOfEnumNameAndDescription<T>() {
			var names = Enum.GetNames(typeof(T));
			var list = new List<EnumNameDescriptionValue>();

			foreach (var name in names) {
				var theEnum = (T)Enum.Parse(typeof(T), name);
				var enumType = theEnum.GetType();
				var field = enumType.GetField(name);
				object[] attrs = field.GetCustomAttributes(true);
				if (attrs.Length > 0) {
					DisplayAttribute da = (DisplayAttribute)attrs[0];
					list.Add(new EnumNameDescriptionValue() {
						Name = da.Name,
						Description = da.Description,
						Value = name
					});
				} else {
					list.Add(new EnumNameDescriptionValue() {
						Name = name.UnCamelCase(),
						Description = name.UnCamelCase(),
						Value = name
					});
				}
			}

			return list;
		}
	}
	public class EnumNameDescriptionValue {
		public string Name { get; set; }
		public string Description { get; set; }
		public string Value { get; set; }
	}
}
