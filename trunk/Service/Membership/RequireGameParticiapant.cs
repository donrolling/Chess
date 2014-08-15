using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service;
using Service.Services;
using Service.Utilities;
using Contracts.Model;
using Service.Interfaces;

namespace Service.Membership {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequireGameParticiapant : AuthorizeAttribute {
		public override void OnAuthorization(AuthorizationContext filterContext) {
			//User user = UserTracker.Current(filterContext.HttpContext);
			//if (user != null && user.Id > 0) {
			//	var gameId = AppPaths.GetGameIdFromPath(filterContext.HttpContext.Request);
			//	if (gameId == 0) {
			//		filterContext.Result = new RedirectResult(AppPaths.NewGameUrl);
			//		return;
			//	}
			//	var hasGame = UserTracker.HasGameByGameId(gameId, user.Id);
			//	if (hasGame) {
			//		return;
			//	}
			//}
			//filterContext.Result = new RedirectResult(AppPaths.NonGameParticipantUrl);
		}
	}
}