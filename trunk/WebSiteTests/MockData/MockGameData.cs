using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Contracts.Model;
using Contracts.Statuses;
using Moq;
using Repository;
using Repository.Responses;
using Service.Interfaces;

namespace WebSiteTests.MockData {
	static class MockGameData {
		public static IGameService GetMockGameDataService(IUserService userService) {			
			var creatorUser = userService.Get(1);
			var opponentUser = userService.Get(2);

			var myGames = getUserGameData(creatorUser, opponentUser);			
			
			var successStatus = ActionResponse.GetActionResponse(ActionType.Create, Status.Success, StatusDetail.New, 1);

			var userGameDataService = new Mock<IGameService>();
			HttpContextBase fakeContext = null;
			userGameDataService.Setup(a => a.AcceptGame(It.IsAny<int>(), It.IsAny<int>())).Returns(successStatus);
			userGameDataService.Setup(a => a.CancelGame(It.IsAny<int>())).Returns(successStatus);
			userGameDataService.Setup(a => a.GetMyGames(fakeContext, It.IsAny<User>())).Returns(myGames);
			return userGameDataService.Object;
		}

		private static List<User_GameData> getUserGameData(User creatorUser, User opponentUser){
			var game1 = new User_GameData { 
				Id = 1, 
				GameDataId = 1, 
				ActiveGame = true, 
				CreatedDate = DateTime.Now, 
				CreatorUser = creatorUser, 
				OpponentUser = opponentUser, 
				CreatorPlayerColor = "w"
			};
			var gameData1 = new GameData { 
				Id = 1, 
				FEN = Chess.Model.FEN.StartingPosition, 
				CreatedDate = DateTime.Now, 
				PGN = string.Empty,
				//UserGameData = uGame1
			};
			game1.GameData = gameData1;

			var game2 = new User_GameData{ 
				Id = 2, 
				ActiveGame = true, 
				CreatedDate = DateTime.Now, 
				CreatorUser = creatorUser, 
				OpponentUser = opponentUser, 
				CreatorPlayerColor = "w"
			};

			var myGames = new List<User_GameData>{
				game1,
				game2
			};

			return myGames;
		}
	}
}
