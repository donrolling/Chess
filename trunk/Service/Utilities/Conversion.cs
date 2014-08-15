using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Web;

namespace Service.Utilities {

	public class Conversion
	{

		public static List<string> OneDimensionalArrayToListOfString(string[] oneDimensionalArray) {
			var list = new List<string>();
			try {
				list = new List<string>(oneDimensionalArray);
			} catch (Exception ex) {
			}
			return list;
		}
		public static List<string> CsvToListOfString(string csv) {
			var list = new List<string>();
			try {
				list = new List<string>(csv.Split(Convert.ToChar(",")));
			} catch (Exception ex) {
			}
			return list;
		}
		public static string ListOfStringToCsv(List<string> list)
		{
			var retVal = new StringBuilder();
			int i = 0;
			foreach (string s in list) {
				if (i > 0) {
					retVal.Append(",");
				}
				retVal.Append(s);
				i += 1;
			}
			return retVal.ToString();
		}
		public static string ListToCsv<T>(List<T> list)
		{
			string retval = list.Aggregate("", (a, next) => string.Concat(a, ",", next.ToString()));
			return retval.Trim(',');
		}
		public static string ArrayToCsv<T>(T[] list)
		{
			string retval = list.Aggregate("", (a, next) => string.Concat(a, ",", next.ToString()));
			return retval.Trim(',');
		}
		public static string ListOfMailAddressesCsv(MailAddressCollection listOfAddresses) {
			var retVal = new StringBuilder();
			int i = 0;
			foreach (MailAddress address in listOfAddresses) {
				if (i > 0) {
					retVal.Append(",");
				}
				retVal.Append(address.Address);
				i += 1;
			}
			return retVal.ToString();
		}

		public static string GetQueryStringValue(HttpRequest request, string key) {
			if (request.QueryString[key] != null) {
				return request.QueryString[key];
			}
			return String.Empty;
		}
		public static string Humanize(bool b) {
			return b ? "Yes" : "No";
		}
		public static string MVCUrlEncode(string s) {
			s = HttpUtility.UrlEncode(s);
			s = s.Replace("%", "!");
			return s;
		}
		public static string MVCUrlDecode(string s) {
			s = s.Replace("!", "%");
			s = HttpUtility.UrlDecode(s);
			return s;
		}

		static object ChangeType(object value, Type conversionType) {
			if (conversionType.IsGenericType &&
				conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) {

				if (value == null)
					return null;

				System.ComponentModel.NullableConverter nullableConverter
					= new System.ComponentModel.NullableConverter(conversionType);

				conversionType = nullableConverter.UnderlyingType;
			}

			return Convert.ChangeType(value, conversionType);
		}

	}

}
