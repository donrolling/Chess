using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using Contracts.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Services;
using WebSiteTests.Tests.Setup;
using Service.Interfaces;
using System.Web.Mvc;
using Service.Membership;
using Contracts.DTO;
using Contracts.Statuses;
using WebSiteTests.MockData;

namespace WebSiteTests.Tests {
	[TestClass]
	public class DatabaseTest {
		public IUserService UserService { get; set; }

		public DatabaseTest() {
			AppConfig.RegisterTestApp();
			UserService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));
		}

		[TestMethod]
		public void PerformGet_GivenConnectionString_ShouldGetData() {
			setupUsers();
		}

		private void setupUsers() { //this will fail if database isn't available, thereby making my assertions
			checkUser(new NewUser{ DisplayName = "Don Rolling", Email = "donrolling@hotmail.com", Password = "password1" });
			checkUser(new NewUser{ DisplayName = "Twin Born Uncle", Email = "thedonof@hotmail.com", Password = "password1" });
			checkUser(new NewUser{ DisplayName = "Donnie Almonds", Email = "don.rolling@intouchsol.com", Password = "password1" });
			checkUser(new NewUser{ DisplayName = "Jimmy Fignewtons", Email = "donrolling@gmail.com", Password = "password1" });
			checkUser(new NewUser{ DisplayName = "Big Daddy Don", Email = "donrolling@yahoo.com", Password = "password1" });
		}

		private void checkUser(NewUser user){
			var userExists = UserService.EmailExists(user.Email);
			if (!userExists) { //create user
				var result = UserService.Create(user);
				Assert.AreEqual(Status.Success, result.Status);
			}
		}
	}
}
