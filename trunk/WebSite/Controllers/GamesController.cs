using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service.Membership;
using Service.Services;
using Contracts.ViewModels;
using Contracts.Model;
using Service.Email;
using Service.Utilities;
using Service.Interfaces;
using Contracts.Statuses;

namespace WebSite.Controllers {
	public partial class GamesController : Controller {
		private IUserService _userService;
		private IGameService _gameService;
		private IChessBoardService _chessBoardService;
		private INotificationService _notificationService;

		public GamesController(IUserService userService, IGameService userGameDataService, IChessBoardService chessBoardService, INotificationService notificationService) {
			this._userService = userService;
			this._gameService = userGameDataService;
			this._chessBoardService = chessBoardService;
			this._notificationService = notificationService;
		}

		[RequireAuthorization]
		public virtual ActionResult NewGame() {
			User user = _userService.Current(this.HttpContext);
			var model = new GameModel { Player1Id = user.Id };
			model.PotentialOpponents = _userService.GetPotentialOpponents(user.Id).Select(a => new SelectListItem { Text = a.DisplayName, Value = a.Id.ToString() });
			return View(model);
		}

		[RequireAuthorization]
		[HttpPost]
		public virtual ActionResult NewGame(GameModel model) {
			if (!ModelState.IsValid) { return View(model); }

			var opponent = _userService.Get(model.Player2Id);
			if (opponent == null) { throw Errors.NoUnknownPlayers; }

			var challenger = _userService.Current(this.HttpContext);
			var response = _gameService.CreateGame(challenger.Id, model.Player1Color, model.Player2Id);
			if (response.Status == Status.Success) {
				_notificationService.NotifyNewChallenge(opponent.Email, challenger.DisplayName, this.HttpContext.Request.Url, response.Id, model.Player2Id);
				return View("NewGameSuccess");
			}
			return View("NewGameFailure", response.ErrorMessage);
		}

		[RequireAuthorization]
		public virtual ActionResult AcceptChallenge(int userGameDataId, int opponentUserId) {
			User user = _userService.Current(this.HttpContext);

			if (user.Id != opponentUserId) {
				return View(MVC.Games.Views.ViewNames.AcceptGameFailure);
			}

			var response = _gameService.AcceptGame(userGameDataId, opponentUserId);
			if (response.Status == Status.Success) {
				return RedirectToAction(MVC.Games.ActionNames.Game, response.Id);
			} else {
				return RedirectToAction(MVC.Games.ActionNames.AcceptGameFailure, response.Id);
			}
		}
		public virtual ActionResult AcceptGameFailure() {
			return View();
		}
		[RequireAuthorization]
		public virtual ActionResult CancelChallenge(int userGameDataId) {
			User user = _userService.Current(this.HttpContext);

			var response = _gameService.CancelGame(userGameDataId);
			if (response.Status == Status.Success) {
				return RedirectToAction(MVC.Games.ActionNames.CancelGameSuccess);
			} else {
				return RedirectToAction(MVC.Games.ActionNames.CancelGameFailure);
			}
		}
		public virtual ActionResult CancelGameSuccess() {
			return View();
		}
		public virtual ActionResult CancelGameFailure() {
			return View();
		}

		[RequireAuthorization]
		[RequireGameParticiapant] //ids that come over as zero  will get redirected by the RequireGameParticiapant attribute.
		public virtual ActionResult Game(int? id) {
			var user = _userService.Current(this.HttpContext);
			if (user == null) {
				throw Errors.UnknownPlayers;
			}

			int gameId = id.Value;

			var userGameData = _gameService.GetUserGameData(gameId);
			if (userGameData == null || userGameData.Id == 0) {
				throw Errors.EmptyGameData;
			}

			//fyi: player 1 and player 2 do not correspond with color
			var player1 = _userService.Get(userGameData.CreatorUserId);
			var player2 = _userService.Get(userGameData.OpponentUserId);
			if (player1 == null || player1.Id == 0 || player2 == null || player2.Id == 0) {
				throw Errors.UnknownPlayers;
			}

			var viewModel = _chessBoardService.GetGameModel(player1, player2, userGameData, user);

			var currentPlayerColor = user.Id == viewModel.Player1Id ? viewModel.Player1Color : viewModel.Player2Color;
			viewModel.CurrentUserPlayerColor = (currentPlayerColor).Substring(0, 1).ToLower();
			return View(viewModel);
		}

		public virtual ActionResult NonGameParticipant() {
			return View();
		}

		public virtual ActionResult Games() {
			var chessGames = _gameService.GetMyGamesAsSelectList(this.HttpContext);
			if (chessGames != null && chessGames.Any()) {
				return PartialView(chessGames);
			}
			return PartialView(new List<SelectListItem>());
		}
		public virtual ActionResult Notifications() {
			var user = _userService.Current(this.HttpContext);
			var chessGames = _userService.GetMyNotifications(user);
			if (chessGames != null && chessGames.Any()) {
				return PartialView(chessGames);
			}
			return PartialView(new List<SelectListItem>());
		}
		public virtual ActionResult PendingGames() {
			var chessGames = _gameService.GetMyGamesAsSelectList(this.HttpContext, false);
			if (chessGames != null && chessGames.Any()) {
				return PartialView(chessGames);
			}
			return PartialView(new List<SelectListItem>());
		}
	}
}
