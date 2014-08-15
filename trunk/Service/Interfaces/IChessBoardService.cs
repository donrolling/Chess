using System;
using Contracts.Model;
using Contracts.Statuses;
using Contracts.ViewModels;
using Repository;
namespace Service.Interfaces {
	public interface IChessBoardService {
		ActionResponse CreateNewGame();
		ActionResponse DeleteGameRequest(int userGameDataId);
		GameData Get(int id);
		ChessBoardViewModel GetOngoingGame(int id);
		ChessBoardViewModel PlayMove(int id, string activeColor, string startSquare, string endSquare);
		ChessBoardViewModel PlayMove(int id, string activeColor, string startSquare, string endSquare, char promoteToPiece);
		ActionResponse Save(int id, GameData GameData);
		GameModel GetGameModel(User player1, User player2, User_GameData userChesGameData, User user);
	}
}
