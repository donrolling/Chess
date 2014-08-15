using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Model;
using Chess.ServiceLayer;

namespace UnitTests {
	[TestClass]
	public class FENTests {
		private static BoardService _boardService;

		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			_boardService = new BoardService();
		}

		[TestMethod]
		public void TranslateFEN() {
			var pos = FEN.StartingPosition.Split(' ')[0];
			var matrix = NotationService.CreateMatrixFromFEN(pos);
			AssertStartingPosition(matrix);
		}
		private void AssertStartingPosition(Dictionary<int, char> matrix){
			Assert.IsTrue(matrix[56] == 'r');
			Assert.IsTrue(matrix[57] == 'n');
			Assert.IsTrue(matrix[58] == 'b');
			Assert.IsTrue(matrix[59] == 'q');
			Assert.IsTrue(matrix[60] == 'k');
			Assert.IsTrue(matrix[61] == 'b');
			Assert.IsTrue(matrix[62] == 'n');
			Assert.IsTrue(matrix[63] == 'r');

			Assert.IsTrue(matrix[48] == 'p');
			Assert.IsTrue(matrix[49] == 'p');
			Assert.IsTrue(matrix[50] == 'p');
			Assert.IsTrue(matrix[51] == 'p');
			Assert.IsTrue(matrix[52] == 'p');
			Assert.IsTrue(matrix[53] == 'p');
			Assert.IsTrue(matrix[54] == 'p');
			Assert.IsTrue(matrix[55] == 'p');

			Assert.IsTrue(matrix[0] == 'R');
			Assert.IsTrue(matrix[1] == 'N');
			Assert.IsTrue(matrix[2] == 'B');
			Assert.IsTrue(matrix[3] == 'Q');
			Assert.IsTrue(matrix[4] == 'K');
			Assert.IsTrue(matrix[5] == 'B');
			Assert.IsTrue(matrix[6] == 'N');
			Assert.IsTrue(matrix[7] == 'R');

			Assert.IsTrue(matrix[8] == 'P');
			Assert.IsTrue(matrix[9] == 'P');
			Assert.IsTrue(matrix[10] == 'P');
			Assert.IsTrue(matrix[11] == 'P');
			Assert.IsTrue(matrix[12] == 'P');
			Assert.IsTrue(matrix[13] == 'P');
			Assert.IsTrue(matrix[14] == 'P');
			Assert.IsTrue(matrix[15] == 'P');
		}

		[TestMethod]
		public void TranslateMatrixToFEN() {
			//var board = _boardService.SetStartPosition(FEN.StartingPosition);
			//var testPos = NotationService.CreateNewFENFromMatrix(board.Matrix, board.ActiveColor);
			//var pos = FEN.StartingPosition.Split(' ')[0];

			//Assert.AreEqual(pos, testPos);
		}

		[TestMethod]
		public void InitializeBoard() {
			Assert.AreEqual("a1", CoordinateService.PositionToCoordinate(0));
			Assert.AreEqual(0, CoordinateService.CoordinateToPosition("a1"));

			Assert.AreEqual("h1", CoordinateService.PositionToCoordinate(7));
			Assert.AreEqual(7, CoordinateService.CoordinateToPosition("h1"));

			Assert.AreEqual("e5", CoordinateService.PositionToCoordinate(36));
			Assert.AreEqual(36, CoordinateService.CoordinateToPosition("e5"));

			Assert.AreEqual("d5", CoordinateService.PositionToCoordinate(35));
			Assert.AreEqual(35, CoordinateService.CoordinateToPosition("d5"));

			Assert.AreEqual("h5", CoordinateService.PositionToCoordinate(39));
			Assert.AreEqual(39, CoordinateService.CoordinateToPosition("h5"));

			Assert.AreEqual("b7", CoordinateService.PositionToCoordinate(49));
			Assert.AreEqual(49, CoordinateService.CoordinateToPosition("b7"));

			Assert.AreEqual("a8", CoordinateService.PositionToCoordinate(56));
			Assert.AreEqual(56, CoordinateService.CoordinateToPosition("a8"));

			Assert.AreEqual("h8", CoordinateService.PositionToCoordinate(63));
			Assert.AreEqual(63, CoordinateService.CoordinateToPosition("h8"));
		}
	}
}
