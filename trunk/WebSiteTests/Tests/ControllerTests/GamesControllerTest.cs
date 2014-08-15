using WebSite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using Contracts.ViewModels;
using Moq;
using Service.Services;
using Service.Interfaces;
using Contracts.Statuses;
using Contracts.Model;
using System.Collections.Generic;
using System.Web;
using Repository;
using Repository.Responses;
using WebSiteTests.MockData;
using WebSiteTests.Tests.Setup;

namespace ChessEngineTests {
	[TestClass()]
	public class GamesControllerTest {
		public GamesController GamesController;
		public IChessBoardService ChessBoardService;
		public IUserService UserService;
		public IGameService GameService;
		public INotificationService NotificationService;

		public GamesControllerTest() {
			AppConfig.RegisterTestApp();
			ChessBoardService = (IChessBoardService)DependencyResolver.Current.GetService(typeof(IChessBoardService));
			NotificationService = (INotificationService)DependencyResolver.Current.GetService(typeof(INotificationService));
			UserService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));
			GameService = (IGameService)DependencyResolver.Current.GetService(typeof(IGameService));
			GamesController = new GamesController(UserService, GameService, ChessBoardService, NotificationService);
		}

		// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
		// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
		// whether you are testing a page, web service, or a WCF service.
		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643/games/acceptchallenge")]
		public void AcceptChallengeTest() {
			int userGameDataId = 0; // TODO: Initialize to an appropriate value
			int opponentUserId = 0; // TODO: Initialize to an appropriate value
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.AcceptChallenge(userGameDataId, opponentUserId);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643")]
		public void CancelChallengeTest() {
			int userGameDataId = 0; // TODO: Initialize to an appropriate value
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.CancelChallenge(userGameDataId);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643")]
		public void GameTest() {			
			Nullable<int> id = new Nullable<int>(); // TODO: Initialize to an appropriate value
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.Game(id);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643")]
		public void GamesTest() {			
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.Games();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643")]
		public void NewGameTest() {			
			GameModel model = null; // TODO: Initialize to an appropriate value
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.NewGame(model);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod()]
		[HostType("ASP.NET")]
		[UrlToTest("http://localhost:49643")]
		public void PendingGamesTest() {			
			ActionResult expected = null; // TODO: Initialize to an appropriate value
			ActionResult actual;
			actual = GamesController.PendingGames();
			Assert.AreEqual(expected, actual);
		}
	}
}
