using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Contracts.Model;
using Repository;
using Repository.Interface;
using Service.Email;
using Service.Interfaces;

namespace Service.Services {
	public class NotificationService : INotificationService {
		private IUserService _userService;
		private INotificationRepository _notificationRepository;
		private EmailManager _emailManager;

		public NotificationService(IUserService userService, INotificationRepository notificationRepository, string smtpServer, string adminEmail, string emailPath, string environment) {
			this._userService = userService;
			this._notificationRepository = notificationRepository;
			this._emailManager = new EmailManager(smtpServer, adminEmail, emailPath, environment);
		}

		public string BuildChallengeLink(Uri uri, int gameId, int opponentId) {
			var challengeUrl = string.Concat(uri.Scheme, "://", uri.Authority, "/Home/AcceptChallenge/", gameId.ToString(), "/", opponentId);
			return challengeUrl;
		}

		public ActionResponse NotifyNewChallenge(string opponentEmail, string challengerDisplayName, Uri uri, int gameId, int opponentId) {
			var challengeUrl = BuildChallengeLink(uri, gameId, opponentId);
			var extendChallengeMessage = Messages.ExtendChallengeMessage(_emailManager.AdminEmail, opponentEmail, challengerDisplayName, challengeUrl);

			var emailReciept = _emailManager.SendEmail(extendChallengeMessage);
			var opponentExists = _userService.EmailExists(opponentEmail);//todo: should do this in one call
			var opponentIdExists = _userService.UserExists(opponentId);
			if (!opponentExists || !opponentIdExists) {
				throw new Exception("User not found.");
			}
			var notification = new Notification {
				Subject = extendChallengeMessage.Subject,
				Message = extendChallengeMessage.Body,
				Deleted = false,
				Read = false,
				CreatedDate = DateTime.Now,
				UserId = opponentId
			};
			return this.Create(notification);
		}

		private ActionResponse Create(Notification notification) {
			return this._notificationRepository.Insert<Notification>(notification);
		}
	}
}