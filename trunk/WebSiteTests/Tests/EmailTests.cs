using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Email;
using Service.Interfaces;
using Service.Services;
using WebSiteTests.MockData;
using WebSiteTests.Tests.Setup;

namespace WebSiteTests {
	[TestClass]
	public class EmailTests {
		public IUserService UserService { get; set; }
		public IGameService GameService { get; set; }
		public INotificationService NotificationService { get; set; }

		public EmailTests() {
			AppConfig.RegisterTestApp();
			UserService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));
			GameService = (IGameService)DependencyResolver.Current.GetService(typeof(IGameService));
			NotificationService = (INotificationService)DependencyResolver.Current.GetService(typeof(INotificationService));
		}

		[TestMethod]
		public void EmailTest() {
			var creatorUser = UserService.GetByEmail("donrolling@hotmail.com");
			Assert.IsNotNull(creatorUser);
			var opponentUser = UserService.GetByEmail("donrolling@gmail.com");
			Assert.IsNotNull(opponentUser);
			var gameId = 5;
			var game = GameService.Get(gameId);
			Assert.IsNotNull(game);
			NotificationService.NotifyNewChallenge(opponentUser.Email, creatorUser.DisplayName, new Uri("http://localhost:49643/"), gameId, opponentUser.Id);
		}
	}
}
