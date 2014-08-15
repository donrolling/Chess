using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestConsole {
	public static class PGNParsing {
		public static string ParsePGNForHTML(string pgn) {
			var regex = @"\d{1,3}\.";
			var splitResult = Regex.Split(pgn, regex);
			if (splitResult != null && splitResult.Any()) {
				StringBuilder result = new StringBuilder(splitResult.Count());
				int i = 1;
				foreach (var item in splitResult) {
					if (!string.IsNullOrEmpty(item)) {
						result.Append(string.Concat(i.ToString(), ".", item.Replace(" ", "&nbsp;")));
						i++;
					}
				}
				return result.ToString();
			}
			return string.Empty;
		}
	}
}
