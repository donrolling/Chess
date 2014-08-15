using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Utilities {
	public static class AppPaths {
		public static string NewGameUrl = string.Concat("~", string.Join("/", "", "Games", "NewGame"));
		public static string NonGameParticipantUrl = string.Concat("~", string.Join("/", "", "Games", "NonGameParticipant"));

		public static int GetGameIdFromPath(HttpRequestBase request){
			var path = request.CurrentExecutionFilePath;
			var afterLastSlash = path.LastIndexOf('/') + 1;
			var idLength = path.Length - (afterLastSlash);
			var gameIdStr = path.Substring(afterLastSlash, idLength);// = "/Home/Game/2014";
			int gameId = 0;
			Int32.TryParse(gameIdStr, out gameId);
			return gameId;
		}
	}
}