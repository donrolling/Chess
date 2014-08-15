using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;
using Repository.Interface;

namespace Repository.Dapper {
	public class DapperGameDataRepository : DapperRepository, IGameDataRepository {
		public DapperGameDataRepository(string connectionString) : base (connectionString) { }

		public User_GameData GetGameRequest(int userGameDataId) {
			return this.Query<User_GameData>("select * from UserGameData where Id = @userGameDataId and ActiveGame = 0", new { userGameDataId = userGameDataId }).FirstOrDefault();
		}


		public User_GameData GetByGameId(int gameId) {
			return this.Query<User_GameData, GameData, User, User, User_GameData>(
										@"select * 
										from [User_GameData] ugd 
											inner join GameDatas gd on ugd.GameDataId = gd.Id
											inner join Users u1 on ugd.CreatorUserId = u1.Id
											inner join Users u2 on ugd.OpponentUserId = u2.Id
										where ugd.GameDataId = @gameId", 
										(ugd, gd, u1, u2) => {
														ugd.GameData = gd;
														ugd.CreatorUser = u1;
														ugd.OpponentUser = u2;
														return ugd;
													},
										new { gameId = gameId },
										"Id,Id,Id"
									).FirstOrDefault();
		}
	}
}
