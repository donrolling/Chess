using System;
using System.Web;
using System.Web.Mvc;
using Contracts.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Interfaces;
using Service.Membership;
using Subtext.TestLibrary;
using WebSite.Controllers;
using WebSiteTests.Tests.Setup;

namespace WebSiteTests.Tests {
	[TestClass]
	public class AccountControllerTests {
		public AccountController AccountController;
		public IChessBoardService ChessBoardService;
		public IUserService UserService;
		public IGameService GameService;
		public INotificationService NotificationService;

		public AccountControllerTests() {
			AppConfig.RegisterTestApp();
			ChessBoardService = (IChessBoardService)DependencyResolver.Current.GetService(typeof(IChessBoardService));
			NotificationService = (INotificationService)DependencyResolver.Current.GetService(typeof(INotificationService));
			UserService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));
			GameService = (IGameService)DependencyResolver.Current.GetService(typeof(IGameService));
			AccountController = new AccountController(UserService);
		}
		
		[TestMethod]
		public void Login() {
			var user = UserService.GetByEmail("thedonof@hotmail.com");
			var loginModel = new LoginModel { UserName = "thedonof@hotmail.com", Password = "password1" };
			var result = AccountController.Login(loginModel, "");
			//var loggedInUser = UserService.Current();
			//Assert.IsNotNull(loggedInUser);
			//Assert.AreEqual(user.Id, loggedInUser.Id);

		}
	}
}
