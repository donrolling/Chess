using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;
using Repository.Interface;

namespace Repository.Dapper {
	public class DapperUserRepository : DapperRepository, IUserRepository {
		public DapperUserRepository(string connectionString) : base(connectionString) { }


		public bool EmailExists(string email) {
			return this.Query<User>("select * from [User] where Email = @email", new { email = email }).Any();
		}

		public User Get(int id) {
			return base.Get<User>(id);
		}

		public bool DisplayNameExists(string displayName) {
			return this.Query<User>("select * from [User] where DisplayName = @displayName", new { displayName = displayName }).Any();
		}

		public User GetByEmail(string email) {
			return this.Query<User>("select * from [User] where Email = @email", new { email = email }).FirstOrDefault();
		}

		public IEnumerable<User_GameData> MyGames(int userId) {
			return this.Query<User_GameData, GameData, User, User, User_GameData>(
							@"select *
								from [User_GameData] ugd 
									inner join [GameData] gd on ugd.GameDataId = gd.Id
									inner join [User] u1 on ugd.CreatorUserId = u1.Id
									inner join [User] u2 on ugd.OpponentUserId = u2.Id
								where ugd.CreatorUserId = @creatorUserId or ugd.OpponentUserId = @opponentUserId",
							(ugd, gd, u1, u2) => {
								ugd.GameData = gd;
								ugd.CreatorUser = u1;
								ugd.OpponentUser = u2;
								return ugd;
							},
							new { creatorUserId = userId, opponentUserId = userId },
							"Id,Id,Id"
						);
		}

		public IEnumerable<User> GetPotentialOpponents(int userId) {
			return this.Query<User>("select * from [User] where Id <> @userId", new { userId = userId });
		}

		public IEnumerable<Notification> GetMyNotifications(int userId) {
			return this.Query<Notification>("select * from [Notification] where UserId = @userId and Deleted = 0", new { userId = userId });
		}

		public Notification GetNotification(int id) {
			return this.Query<Notification>("select * from [Notification] where Id = @id and Deleted = 0", new { id = id }).SingleOrDefault();
		}

		public bool HasGame(int gameId) {
			return this.Query<User_GameData>("select * from [User_GameData] where Id = @gameId", new { gameId = gameId }).Any();
		}

		public bool HasGame(int gameId, int userId) {
			var query = new { gameDataId = gameId, OpponentUserId = userId, CreatorUserId = userId};
			return this.Query<User_GameData>("select * from [User_GameData] where GameDataId = @gameDataId and (OpponentUserId = @opponentUserId or CreatorUserId = @creatorUserId)", query).Any();
		}

		public bool UserExists(int id) {
			return this.Query<User>("select * from [User] where Id = @id", new { id = id }).Any();
		}
	}
}
