using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using Contracts.DTO;
using Contracts.Model;
using Contracts.Statuses;
using Omu.ValueInjecter;
using Repository;
using Repository.Interface;
using Repository.Responses;
using Service.Interfaces;
using Service.Utilities;

namespace Service.Services {
	public class UserService : IUserService {
		private IUserRepository _userRepository;

		private string _sessionUserKey = "SessionUserKey";
		private int _minutesCookieIsValid = 900;
		private string _cookieName = FormsAuthentication.FormsCookieName;
		private bool _isInitialized = false;

		public UserService(IUserRepository userRepository) {
			_userRepository = userRepository;
		}

		public User Get(int id) {
			var user = _userRepository.Get(id);
			return user;
		}
		
		public ActionResponse Create(NewUser user) {
			return this.Create(user.DisplayName, user.Email, user.Password);
		}
		
		public ActionResponse Create(string displayName, string email, string password) {
			var emailExists = _userRepository.EmailExists(email);
			if (emailExists) {
				return ActionResponse.GetActionResponse(ActionType.Create, Status.Failure, StatusDetail.Duplicate);
			}

			var displayNameExists = _userRepository.DisplayNameExists(displayName);
			if (displayNameExists) {
				return ActionResponse.GetActionResponse(Repository.Responses.ActionType.Create, Status.Failure, StatusDetail.Duplicate, 0, "This display name is already being used.");
			}

			var hashedPasswordAndSalt = PasswordManager.GetHashedPasswordAndSalt(password);
			var user =	new User{
								DisplayName = displayName,
								Email = email,
								Password = hashedPasswordAndSalt.HashedPassword,
								Salt = hashedPasswordAndSalt.Salt,
							};
			var response = _userRepository.Insert(user);
			return response;
		}

		public ActionResponse Save(User user) {
			var isNew = user.Id == 0 ? true : false;
			if(isNew){
				throw new Exception("Don't create new users using the Save method.");
			}

			var result = _userRepository.Update(user);
			return result;
		}

		public LoginStatus Login(string email, string password) {
			var user = _userRepository.GetByEmail(email);
			if(user == null){
				return LoginStatus.GetLoginStatus(LoginStatus.LoginStatuses.UsernameNotFound);
			}

			var authenticates = PasswordManager.ComparePasswordWithHashedPassword(password, user.Salt, user.Password);
			if (authenticates) {
				return LoginStatus.GetLoginStatus(LoginStatus.LoginStatuses.Success, user.Id);
			} else {
				return LoginStatus.GetLoginStatus(LoginStatus.LoginStatuses.Failure);
			}
		}
		
		public void AutoLoginUser(string username, string environment) {
			if (environment != "Local") { return; }
			var user = this.GetByEmail(username);
			if (user == null) { return; }
			try { //this will fail in unit tests, so throw the error away
				FormsAuthentication.SetAuthCookie(user.Id.ToString(), true);
			} catch { }		
		}

		public IEnumerable<User_GameData> MyGames(User user){
			var myGames = _userRepository.MyGames(user.Id);
			return myGames;
		}

		public User GetByEmail(string email) {
			var user = _userRepository.GetByEmail(email);
			return user;
		}
		
		public IEnumerable<User> GetPotentialOpponents(int userId){
			var users = _userRepository.GetPotentialOpponents(userId);
			return users;
		}

		public IEnumerable<Notification> GetMyNotifications(User user) {
			if (user != null && user.Id != 0) {
				var userId = new { userId = user.Id.ToString() };
				var notifications = _userRepository.GetMyNotifications(user.Id);
				return notifications;
			}
			return null;
		}
		
		public Notification GetMyNotification(int id) {
			var notification = _userRepository.GetNotification(id);
			return notification;
		}

		public bool HasGame(int gameId) {
			var userGameData = _userRepository.HasGame(gameId);
			return userGameData;
		}

		public bool HasGame(int gameId, int userId) {
			var userGameData = _userRepository.HasGame(gameId, userId);
			return userGameData;
		}

		public bool EmailExists(string email) {
			return _userRepository.EmailExists(email);
		}

		public bool UserExists(int id) {
			return _userRepository.UserExists(id);
		}

		public User Current(HttpContextBase context) { //todo: make this work better...so many issues here
			var userId = 0;
			try {
				userId = getUserIdFromCookie(context);
			} catch (Exception) { }
			if (userId > 0) {
				var user = this.Get(userId);
				return user;
			}
			return null;
		}

		private int getUserIdFromCookie(HttpContextBase context) {
			var cookie = context != null ? context.Request.Cookies[FormsAuthentication.FormsCookieName] : null;
			if (cookie == null) {
				return 0;
			}

			FormsAuthenticationTicket ticket = null;
			try {
				if (cookie.Value.Length == 0) {
					return 0;
				}
				ticket = FormsAuthentication.Decrypt(cookie.Value);
			} catch { }
			if (ticket == null) {
				return 0;
			}

			int userId = 0;
			int.TryParse(ticket.Name, out userId);
			return userId; //zero will indicate guest user
		}


		public void Logoff(HttpContextBase httpContextBase) {
			throw new NotImplementedException();
		}
	}
}