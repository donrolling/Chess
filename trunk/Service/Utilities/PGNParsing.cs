using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Chess.ServiceLayer;

namespace Service.Utilities {
	public static class PGNParsing {
		public static string ParsePGNForHTML(string pgn) {
			if(string.IsNullOrEmpty(pgn)){ return string.Empty; }

			var splitResult = PGNService.PGNSplit(pgn);
			if (splitResult != null && splitResult.Any()) {
				StringBuilder result = new StringBuilder(splitResult.Count());
				int moveNumber = 1;
				var tag1 = "<span class='chessMove'><span class='moveNumber'>";
				var tag2 = "</span><span class='moveText'>";
				var tag3 = "</span></span>";
				foreach (var item in splitResult) {
					if (!string.IsNullOrEmpty(item)) {
						var trimmedItem = item.Trim();
						var parsedItem = string.Concat("{0}", moveNumber.ToString(), ".{1}", trimmedItem.Replace(" ", "&nbsp;"), "{2} ");
						parsedItem = string.Format(parsedItem, tag1, tag2, tag3);
						result.Append(parsedItem);
						moveNumber++;
					}
				}
				return result.ToString();
			}
			return string.Empty;
		}
	}
}