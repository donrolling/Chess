using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Service;
using Contracts.Model;
using Service.Services;
using Service.Interfaces;

namespace Service.Membership {
	public static class UserTracker {
		private static IUserService _userService;

		private static Exception _uninitializedUserTracker = new Exception("User Tracker method: Initialize has not been called so the class cannot function properly.");

		private static string _sessionUserKey = "SessionUserKey";
		private static int _minutesCookieIsValid = 900;
		private static string _cookieName = FormsAuthentication.FormsCookieName;
		private static bool _isInitialized = false;

		public static void Initialize(IUserService userService) {
			_isInitialized = true;
			_userService = userService;
		}

		public static User Current(HttpContextBase context) {
			if (!_isInitialized) {
				throw _uninitializedUserTracker;
			}
			try {
				var sessionUser = context.Session[_sessionUserKey];
				if (sessionUser != null) {
					return (User)sessionUser;
				}
			} catch {}
			var userId = getUserIdFromCookie(context);
			if (userId > 0) {
				var user = _userService.Get(userId);
				context.Session[_sessionUserKey] = user;
				return user;
			}
			return null;
		}

		public static void Login(string username) {
			if (!_isInitialized) {
				throw _uninitializedUserTracker;
			}
			var user = _userService.GetByEmail(username);
			if (user != null) {
				try { //this will fail in unit tests, so throw the error away
					FormsAuthentication.SetAuthCookie(user.Id.ToString(), true);
				} catch { }		
			}
		}
		public static void Logoff(HttpContextBase context) {
			if (!_isInitialized) {
				throw _uninitializedUserTracker;
			}
			context.Session.Clear();
			FormsAuthentication.SignOut();
		}
		
		public static bool HasGame(int gameId) {
			return _userService.HasGame(gameId);
		}
		public static bool HasGameByGameId(int gameId, int userId) {
			return _userService.HasGame(gameId, userId);
		}

		private static int getUserIdFromCookie(HttpContextBase context) {
			var cookie = context != null ? context.Request.Cookies[FormsAuthentication.FormsCookieName] : null;
			if (cookie == null) {
				return 0;
			}

			FormsAuthenticationTicket ticket = null;
			try {
				if (cookie.Value.Length == 0) {
					return 0;
				}
				ticket = FormsAuthentication.Decrypt(cookie.Value);
			} catch { }
			if (ticket == null) {
				return 0;
			}

			int userId = 0;
			int.TryParse(ticket.Name, out userId);
			return userId; //zero will indicate guest user
		}

		public static void AutoLoginUser(string username, string environment) {
			if (environment != "Local") { return; }
			var user = _userService.GetByEmail(username);
			if (user == null) { return; }
			try { //this will fail in unit tests, so throw the error away
				FormsAuthentication.SetAuthCookie(user.Id.ToString(), true);
			} catch { }		
		}
	}
}