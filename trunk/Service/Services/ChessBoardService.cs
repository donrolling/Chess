using System.Collections.Generic;
using System.Data;
using System.Linq;
using Chess;
using Chess.Model;
using Chess.ServiceLayer;
using Contracts.Model;
using Contracts.Statuses;
using Contracts.ViewModels;
using Omu.ValueInjecter;
using Repository;
using Repository.Dapper.Contrib;
using Repository.Interface;
using Repository.Responses;
using Service.Interfaces;
using Service.Utilities;

namespace Service.Services {
	public class ChessBoardService : IChessBoardService {
		private IGameDataRepository _gameDataRepositoryRepository;

		public ChessBoardService(IGameDataRepository gameDataRepositoryRepository) {
			_gameDataRepositoryRepository = gameDataRepositoryRepository;
		}

		public GameData Get(int id) {
			var savedGame = _gameDataRepositoryRepository.Get<GameData>(id);
			return savedGame;
		}
		
		public ActionResponse Save(int id, GameData saveGame) {
			var actionType = id == 0 ? ActionType.Create : ActionType.Update;
			if(actionType == ActionType.Create){
				return _gameDataRepositoryRepository.Insert(saveGame);
			} else {
				return _gameDataRepositoryRepository.Update(saveGame);
			}
		}

		public ActionResponse CreateNewGame() {
			var game = new Game();
			var gameData = ChessBoardService.getGameDataFromGame(0, game);
			var response = this.Save(0, gameData);
			return response;
		}

		public ActionResponse DeleteGameRequest(int userGameDataId) {
			var gameRequest = _gameDataRepositoryRepository.Get<User_GameData>(userGameDataId);
			if (gameRequest == null) {
				return ActionResponse.GetActionResponse(ActionType.Delete, Status.Failure, StatusDetail.ItemNotFound,  userGameDataId, "Cannot find game. Unable to delete.");
			}
			if (gameRequest.ActiveGame == true) {
				return ActionResponse.GetActionResponse(ActionType.Delete, Status.Failure, StatusDetail.Error, userGameDataId, "Cannot delete an active game. Please resign before deleting.");
			}
			var result = _gameDataRepositoryRepository.Delete<User_GameData>(gameRequest.Id);
			return result;
		}
		public ChessBoardViewModel GetOngoingGame(int id) {
			var GameData = this.Get(id);
			var chessBoardViewModel = GetChessBoardViewModel(id, GameData);
			return chessBoardViewModel;
		}
		public ChessBoardViewModel PlayMove(int id, string activeColor, string startSquare, string endSquare) {
			return PlayMove(id, activeColor, startSquare, endSquare, PGNService.NullPiece);
		}
		public ChessBoardViewModel PlayMove(int id, string activeColor, string startSquare, string endSquare, char promoteToPiece) {
			GameData GameData = this.Get(id);
			Game game = GetGameFromGameData(GameData);
			string pgnMove = getPGNMoveFromStartEndSquares(game, startSquare, endSquare, promoteToPiece);
			return playMove(id, game, GameData, activeColor, pgnMove);
		}

		public static ChessBoardViewModel GetChessBoardViewModel(int gameId, GameData GameData) {
			ChessBoardViewModel chessBoardViewModel = new ChessBoardViewModel();
			Game game = GetGameFromGameData(GameData);
			chessBoardViewModel.InjectFrom(game.Board);
			chessBoardViewModel.PGN = PGNParsing.ParsePGNForHTML(chessBoardViewModel.PGN);
			chessBoardViewModel.GameId = gameId;
			return chessBoardViewModel;
		}
		public static Game GetGameFromGameData(GameData GameData) {
			Game game = new Game();
			if (!string.IsNullOrEmpty(GameData.PGN)) {
				var pgnData = Chess.ServiceLayer.PGNService.PGNSplit(GameData.PGN, true);
				if (pgnData != null && pgnData.Any()) {
					foreach (var pgn in pgnData) {
						if (pgn.Contains(".")) {
							continue;
						}
						game.Move(game.Board.ActiveChessTypeColor, pgn);
					}
				}
			}
			return game;
		}
		public GameModel GetGameModel(User player1, User player2, User_GameData userGameData, User user) {
			//fyi: player 1 and player 2 do not correspond with color, i.e. player 1 can be white or black
			var gameModel = new GameModel {
				Id = userGameData.GameDataId.Value,
				Player1Id = player1.Id,
				Player1Name = player1.DisplayName,
				Player2Id = player2.Id,
				Player2Name = player2.DisplayName,
				EncryptedUserId = new SimpleAES().EncryptToString(user.Id.ToString())
			};
			gameModel.Player1Color = userGameData.CreatorPlayerColor;
			gameModel.Player2Color = userGameData.CreatorPlayerColor == ChessType.Color.White.ToString() ? ChessType.Color.Black.ToString() : ChessType.Color.White.ToString();
			gameModel.CreatedDate = userGameData.CreatedDate;
			//gameModel.FEN = userGameData.GameData.FEN;
			return gameModel;
		}

		private ChessBoardViewModel playMove(int id, Game game, GameData GameData, string activeColor, string pgnMove) {
			var activeChessTypeColor = activeColor == "w" ? ChessType.Color.White : ChessType.Color.Black;
			game.Move(activeChessTypeColor, pgnMove);
			var updatedGameData = getGameDataFromGame(id, game);
			if (game.Board.MoveSuccess) {
				this.Save(id, updatedGameData);
			}
			ChessBoardViewModel chessBoardViewModel = GetChessBoardViewModel(id, updatedGameData);
			chessBoardViewModel.MoveFailureMessage = game.Board.MoveFailureMessage;
			chessBoardViewModel.MoveSuccess = game.Board.MoveSuccess;
			return chessBoardViewModel;
		}
		private static GameData getGameDataFromGame(int id, Game game) {
			GameData GameData = new GameData();
			GameData.InjectFrom(game.Board);
			GameData.Id = id;
			return GameData;
		}
		private static string getPGNMoveFromStartEndSquares(Game game, string startSquare, string endSquare, char promoteToPiece) {
			Dictionary<int, char> matrix = game.Board.Matrix;
			Dictionary<int, List<int>> allAttacks = game.Board.AllAttacks;
			ChessType.Color playerColor = game.Board.ActiveChessTypeColor;
			string pgnMove = PGNService.SquarePairToPGNMove(matrix, allAttacks, playerColor, startSquare, endSquare, promoteToPiece);
			return pgnMove;
		}
	}
}