using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service.Membership;
using Service.Interfaces;
using Contracts.Statuses;
using Contracts.Model;
using System.Data;
using Repository;
using Repository.Interface;
using Repository.Responses;

namespace Service.Services {
	public class GameService : IGameService {
		private IGameDataRepository _gameDataRepository;
		private IChessBoardService _chessBoardService;
		private IUserService _userService;

		public GameService(IGameDataRepository repository, IChessBoardService chessBoardService, IUserService userService) {
			this._gameDataRepository = repository;
			this._chessBoardService = chessBoardService;
			this._userService = userService;
		}

		public ActionResponse CreateGame(int creatorUserId, string creatorPlayerColor, int opponentUserId) {
			var isNew = creatorUserId == 0 || opponentUserId  == 0 ? true : false;
			if (isNew) {
				throw new Exception("Can't create a new game with an unknown user!");
			}

			var opponent = _userService.Get(opponentUserId);
			if (opponent == null) {
				throw new Exception("Can't create a new game with an unknown opponent!");
			}

			var gameRequest = new User_GameData();
			gameRequest.CreatorUserId = creatorUserId;
			gameRequest.CreatorPlayerColor = creatorPlayerColor;
			gameRequest.OpponentUserId = opponentUserId;
			gameRequest.ActiveGame = false;
			gameRequest.CreatedDate = DateTime.Now;

			var result = _gameDataRepository.Insert<User_GameData>(gameRequest);

			return result;
		}
		public ActionResponse AcceptGame(int userGameDataId, int opponentUserId) {
			var isNew = opponentUserId == 0 ? true : false;
			if (isNew) {
				throw new Exception("Can't create a new game with an invalid user!");
			}

			var gameRequest = _gameDataRepository.GetGameRequest(userGameDataId);
			if (gameRequest == null) {
				return ActionResponse.GetActionResponse(ActionType.Get, Status.ItemNotFound, StatusDetail.ItemNotFound, userGameDataId);
			}

			var result = _chessBoardService.CreateNewGame();
			if (result.Status == Status.Success) {
				//todo: need to update gameRequest, right?
				gameRequest.ActiveGame = true;
				gameRequest.GameDataId = result.Id;				
			}

			return result;
		}
		public ActionResponse CancelGame(int userGameDataId) {
			var ActionResponse = _chessBoardService.DeleteGameRequest(userGameDataId);
			return ActionResponse;
		}
		
		public GameData Get(int gameId) {
			var userGameData = _gameDataRepository.Get<GameData>(gameId);
			return userGameData;
		}
		public User_GameData GetUserGameData(int gameId) {
			var userGameData = _gameDataRepository.Get<User_GameData>(gameId);
			return userGameData;
		}
		
		public List<SelectListItem> GetMyGamesAsSelectList(HttpContextBase context, bool activeGames = true) {
			var user = _userService.Current(context);
			var chessGames = activeGames ? GetMyGames(context, user) : GetMyPendingGames(context, user);
			if (chessGames != null && chessGames.Any()) {
				var viewData = GameDataToSelectList(chessGames, user, !activeGames);
				return viewData;
			}
			return new List<SelectListItem>();
		}
		public IEnumerable<User_GameData> GetMyGames(HttpContextBase context, User user) {
			if (user != null && user.Id != 0) {
				var chessGames = _userService.MyGames(user).Where(a => a.GameDataId != null);
				return chessGames;
			}
			return null;
		}
		public IEnumerable<User_GameData> GetMyPendingGames(HttpContextBase context, User user) {
			if (user != null && user.Id != 0) {
				var chessGames = _userService.MyGames(user).Where(a => a.GameDataId == null);
				return chessGames;
			}
			return null;
		}
		public List<SelectListItem> GameDataToSelectList(IEnumerable<User_GameData> chessGames, User user, bool isPendingGame = false) {
			var myGames = new List<SelectListItem>();

			foreach (var chessGame in chessGames) {
				var gameTitle = string.Concat(chessGame.CreatorUser.DisplayName, " vs. ", chessGame.OpponentUser.DisplayName, " - ", chessGame.Id.ToString());
				var game = new SelectListItem { Text = gameTitle };
				if (isPendingGame){
					if (chessGame.OpponentUserId == user.Id) {
						game.Value = string.Join(";", chessGame.Id.ToString(), user.Id.ToString());
					} else {
						game.Value = string.Join(";", chessGame.Id.ToString(), "0");
					}
				}else{
					game.Value = chessGame.GameDataId.ToString();
				}
				myGames.Add(game);
			}

			return myGames;
		}
	}
}