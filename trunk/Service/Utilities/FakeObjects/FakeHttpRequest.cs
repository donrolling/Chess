using System;
using System.Collections.Specialized;
using System.Web;

namespace Service.Utilities.FakeObjects {

	public class FakeHttpRequest : HttpRequestBase {
		private readonly NameValueCollection _formParams;
		private readonly NameValueCollection _queryStringParams;
		private readonly HttpCookieCollection _cookies;
		private readonly Uri _url;

		public FakeHttpRequest(NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies, Uri url) {
			_formParams = formParams;
			_queryStringParams = queryStringParams;
			_cookies = cookies;
			_url = url;
		}

		public override Uri Url {
			get {
				return _url;
			}
		}

		public override NameValueCollection Form {
			get {
				return _formParams;
			}
		}

		public override NameValueCollection QueryString {
			get {
				return _queryStringParams;
			}
		}

		public override HttpCookieCollection Cookies {
			get {
				return _cookies;
			}
		}
	}
}