using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Utilities {
	public static class StringExtensions {
		public static string UnCamelCase(this string value) {
			if (!string.IsNullOrEmpty(value)) {
				StringBuilder sb = new StringBuilder();
				foreach (char c in value) {
					if (char.IsLower(c)) {
						sb.Append(c);
					} else {
						sb.Append(" " + c);
					}
				}
				return sb.ToString().Trim();
			}
			return value;
		}
		/// <summary>
		/// Cleans special characters from a string.
		/// </summary>
		/// <param name="value">Source string</param>
		/// <param name="exclusionString">Characters that should be excluded.</param>
		/// <param name="inclusionString">Characters that should be allowed.</param>
		/// <returns>Clean string</returns>
		public static string Cleanse(this string value, string exclusionString = null, string inclusionString = null) {
			if (!string.IsNullOrEmpty(value)) {
				bool[] _lookup = new bool[65535];

				//keep numbers and all alphabetical values by default
				for (char c = '0'; c <= '9'; c++)
					_lookup[c] = true;
				for (char c = 'A'; c <= 'Z'; c++)
					_lookup[c] = true;
				for (char c = 'a'; c <= 'z'; c++)
					_lookup[c] = true;
				//keep spaces by default
				_lookup[' '] = true;

				if (!string.IsNullOrEmpty(exclusionString)) {
					foreach (char c in exclusionString) {
						_lookup[c] = false;
					}
				}
				if (!string.IsNullOrEmpty(inclusionString)) {
					foreach (char c in inclusionString) {
						_lookup[c] = true;
					}
				}

				char[] buffer = new char[value.Length];
				int index = 0;
				foreach (char c in value) {
					if (_lookup[c]) {
						buffer[index] = c;
						index++;
					}
				}
				return new string(buffer, 0, index);
			}
			return string.Empty;
		}
		public static string CamelCase(this string value) {
			if (!string.IsNullOrEmpty(value)) {
				return value.Replace(" ", "");
			}
			return value;
		}
		public static IEnumerable<String> SplitInParts(this String value, Int32 partLength) {
			if (value == null)
				throw new ArgumentNullException("SplitInParts method cannot be called with attribute null string.");
			if (partLength <= 0)
				throw new ArgumentException("Part length has to be positive.", "partLength");
			for (var i = 0; i < value.Length; i += partLength)
				yield return value.Substring(i, Math.Min(partLength, value.Length - i));
		}
		public static byte[] ToByteArray(this string value) {
			if (value.Length < 8) {
				throw new Exception("String must have at least 8 characters.");
			}
			byte[] bytes = Encoding.ASCII.GetBytes(value.Substring(0, 8));
			return bytes;
		}
	}
}
