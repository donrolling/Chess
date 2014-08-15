using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace Service.Utilities.FakeObjects {
	public class FakeHttpContext : HttpContextBase {
		private readonly FakePrincipal _principal;
		private readonly NameValueCollection _formParams;
		private readonly NameValueCollection _queryStringParams;
		private readonly HttpCookieCollection _cookies;
		private readonly SessionStateItemCollection _sessionItems;
		private readonly Uri _url;

		private Dictionary<object, object> _items = new Dictionary<object, object>();
		public override IDictionary Items { get { return _items; } }

		public FakeHttpContext(FakePrincipal principal, NameValueCollection formParams, NameValueCollection queryStringParams, HttpCookieCollection cookies, SessionStateItemCollection sessionItems, Uri url) {
			_principal = principal;
			_formParams = formParams;
			_queryStringParams = queryStringParams;
			_cookies = cookies;
			_sessionItems = sessionItems;
			_url = url;
		}

		public override HttpRequestBase Request {
			get {
				return new FakeHttpRequest(_formParams, _queryStringParams, _cookies, _url);
			}
		}

		public override IPrincipal User {
			get {
				return _principal;
			}
			set {
				throw new System.NotImplementedException();
			}
		}

		public override HttpSessionStateBase Session {
			get {
				return new FakeHttpSessionState(_sessionItems);
			}
		}

	}
}
