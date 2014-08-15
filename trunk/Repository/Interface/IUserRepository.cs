using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;

namespace Repository.Interface {
	public interface IUserRepository : IRepository {
		//ActionResponse Insert(User user);
		//ActionResponse Update(User user);

		bool EmailExists(string email);

		User Get(int id);

		bool DisplayNameExists(string displayName);

		User GetByEmail(string email);

		IEnumerable<User_GameData> MyGames(int userId);

		IEnumerable<User> GetPotentialOpponents(int userId);

		IEnumerable<Notification> GetMyNotifications(int userId);

		Notification GetNotification(int id);

		bool HasGame(int gameId);

		bool HasGame(int gameId, int userId);

		bool UserExists(int id);
	}
}
