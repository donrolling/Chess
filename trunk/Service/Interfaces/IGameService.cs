using System;
using System.Collections.Generic;
using Contracts.Statuses;
using Contracts.Model;
using System.Web;
using Repository;

namespace Service.Interfaces {
	public interface IGameService {
		ActionResponse AcceptGame(int userGameDataId, int opponentUserId);
		ActionResponse CancelGame(int userGameDataId);
		List<System.Web.Mvc.SelectListItem> GameDataToSelectList(IEnumerable<User_GameData> chessGames, User user, bool isPendingGame = false);
		ActionResponse CreateGame(int creatorUserId, string creatorPlayerColor, int opponentUserId);
		GameData Get(int gameId);
		User_GameData GetUserGameData(int gameId);
		IEnumerable<User_GameData> GetMyGames(HttpContextBase context, User user);
		List<System.Web.Mvc.SelectListItem> GetMyGamesAsSelectList(HttpContextBase context, bool activeGames = true);
		IEnumerable<User_GameData> GetMyPendingGames(HttpContextBase context, User user);
	}
}