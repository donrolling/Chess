using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;

namespace Repository.Helpers {
	public class TypeInfo<T> where T : class {
		public bool IsNumeric { get; set; }
		public bool IsBoolean { get; set; }
		public bool IsString { get; set; }
		public bool IsByte { get; set; }
		public bool IsDateTime { get; set; }
		
		private object value;

		public TypeInfo(PropertyInfo propertyInfo, T entity) {
			var typeCode = Type.GetTypeCode(propertyInfo.PropertyType);
			this.value = propertyInfo.GetValue(entity, null);

			switch (typeCode) {
				case TypeCode.DBNull:
				case TypeCode.Empty:
				case TypeCode.Object:
					//wierd, don't know what to do with these
					break;
				
				case TypeCode.Boolean:
					this.IsBoolean = true;
					break;

				case TypeCode.DateTime:
					this.IsDateTime = true;
					break;

				case TypeCode.SByte:
				case TypeCode.Byte:
					this.IsByte = true;
					break;
				
				case TypeCode.Char:
				case TypeCode.String:
					this.IsString = true;
					break;
				
				case TypeCode.Decimal: //is decimal the same as number?
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					this.IsNumeric = true;
					break;
			}
		}
		
		public string GetValueAsSQLString(){
			if (this.IsDateTime) {

			}
			if (this.IsDateTime) {

			}
			if (this.IsDateTime) {

			}
			if (this.IsDateTime) {

			}
			if (this.IsDateTime) {

			}
			return "";
		}
		private T getValue<T>() where T : struct{
			T item = type<T>(this.value);
			return item;
		}

		private static T type<T>(object value) {
			T retval = default(T);
			if (value != null) {
				var converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null) {
					return (T)converter.ConvertFromString(value.ToString());
				}
			} else { 
				throw new Exception("Configuration value is null");
			}
			return retval;
		}
	}
}
