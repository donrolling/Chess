using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Utilities {
	public static class LinkBuilder {
		/// <summary>
		/// Builds a link that goes to the current running site, regardless of environment.
		/// </summary>
		/// <param name="requestUri">this.HttpContext.Request.Url should about do it.</param>
		/// <param name="rightPart">This is everything but the left part - ex. '/Home/Stuff'</param>
		/// <returns></returns>
		public static string Build(Uri requestUri, string rightPart){
			var result = string.Concat(requestUri.Scheme, "://", requestUri.Authority, rightPart);
			return result;
		}
	}
}