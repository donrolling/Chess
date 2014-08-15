using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;
using Moq;
using Service.Interfaces;

namespace WebSiteTests.MockData {
	static class MockUserData {	
		private static User _creatorUser = new User { Id = 1, Email = "donrolling@gmail.com", CreatedDate = DateTime.Now, DisplayName = "Gmail" };
		private static User _opponentUser = new User { Id = 2, Email = "donrolling@hotmail.com", CreatedDate = DateTime.Now, DisplayName = "Hotmail" };

		public static IUserService GetMockUserService() {
			var userService = new Mock<IUserService>();
			userService.Setup(a => a.Get(It.IsAny<int>())).Returns(_creatorUser);
			userService.Setup(a => a.Get(1)).Returns(_creatorUser);
			userService.Setup(a => a.Get(2)).Returns(_opponentUser);
			userService.Setup(a => a.GetByEmail(_creatorUser.Email)).Returns(_creatorUser);
			userService.Setup(a => a.GetByEmail(_opponentUser.Email)).Returns(_opponentUser);
			return userService.Object;
		}
	}
}
