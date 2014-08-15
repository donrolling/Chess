using System;
using Contracts.Model;
using Contracts.Statuses;
using System.Collections.Generic;
using Repository.Interface;
using Repository;
using System.Web;
namespace Service.Interfaces {
	public interface IUserService {
		ActionResponse Create(string displayName, string email, string password);
		User Get(int id);
		User GetByEmail(string email);
		bool EmailExists(string email);
		IEnumerable<Contracts.Model.User> GetPotentialOpponents(int userId);
		LoginStatus Login(string email, string password);
		void AutoLoginUser(string username, string environment);
		IEnumerable<Contracts.Model.User_GameData> MyGames(Contracts.Model.User user);
		ActionResponse Create(Contracts.DTO.NewUser user);
		ActionResponse Save(Contracts.Model.User user);
		IEnumerable<Notification> GetMyNotifications(User user);
		Notification GetMyNotification(int id);
		bool HasGame(int gameId);
		bool HasGame(int gameId, int userId);
		bool UserExists(int id);
		User Current(HttpContextBase context);

		void Logoff(HttpContextBase httpContextBase);
	}
}