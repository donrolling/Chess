using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Statuses;
using Moq;
using Repository;
using Repository.Responses;
using Service.Interfaces;

namespace WebSiteTests.MockData {
	static class MockChessBoard {
		public static IChessBoardService GetMockChessBoardService() {
			var chessBoardService = new Mock<IChessBoardService>();
			var successStatus = ActionResponse.GetActionResponse(ActionType.Create, Status.Success, StatusDetail.New, 1);
			chessBoardService.Setup(a => a.CreateNewGame()).Returns(successStatus);
			return chessBoardService.Object;
		}
	}
}
