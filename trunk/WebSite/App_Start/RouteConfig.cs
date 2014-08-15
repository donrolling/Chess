using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite {
	public class RouteConfig {
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "CancelChallenge",
				url: "Games/CancelChallenge/{userGameDataId}",
				defaults: new { controller = "Games", action = "CancelChallenge" }
			);
			routes.MapRoute(
				name: "AcceptChallenge",
				url: "Games/AcceptChallenge/{userGameDataId}/{opponentUserId}",
				defaults: new { controller = "Games", action = "AcceptChallenge" }
			);
			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}