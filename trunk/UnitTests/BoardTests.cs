using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.ServiceLayer;
using Chess.Model;

namespace UnitTests {
	[TestClass]
	public class BoardTests {
		private static BoardService _boardService;

		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			_boardService = new BoardService();
		}

		[TestMethod]
		public void UpdateTest() {
			//set up default position and test that it is correct
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);
			AssertStartingPosition(board.Matrix);
			//now change the board and test if the change took place
			//testing position
			//testing en passant target square
			//1. e4
			var pos1 = CoordinateService.CoordinateToPosition("e2");
			var pos2 = CoordinateService.CoordinateToPosition("e4");
			board = _boardService.UpdateBoard(board, ChessType.Color.White, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix[28] == 'P');
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", board.FEN);
			//1. e4 c5
			//testing position
			//testing en passant target square
			//testing fullmove number
			pos1 = CoordinateService.CoordinateToPosition("c7");
			pos2 = CoordinateService.CoordinateToPosition("c5");
			board = _boardService.UpdateBoard(board, ChessType.Color.Black, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix[34] == 'p');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2", board.FEN);
			//1. e4 c5 2. Nc3
			//testing the halfmove clock
			pos1 = CoordinateService.CoordinateToPosition("b1");
			pos2 = CoordinateService.CoordinateToPosition("c3");
			board = _boardService.UpdateBoard(board, ChessType.Color.White, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix[18] == 'N');
			Assert.AreEqual("rnbqkbnr/pp1ppppp/8/2p5/4P3/2N5/PPPP1PPP/R1BQKBNR b KQkq - 1 2", board.FEN);

			//testing castle availability
			//1. e4 c5 2. Nc3 b5
			pos1 = CoordinateService.CoordinateToPosition("b7");
			pos2 = CoordinateService.CoordinateToPosition("b5");
			board = _boardService.UpdateBoard(board, ChessType.Color.Black, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/2N5/PPPP1PPP/R1BQKBNR w KQkq b6 0 3", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3
			pos1 = CoordinateService.CoordinateToPosition("b2");
			pos2 = CoordinateService.CoordinateToPosition("b3");
			board = _boardService.UpdateBoard(board, ChessType.Color.White, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("rnbqkbnr/p2ppppp/8/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR b KQkq - 0 3", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6
			pos1 = CoordinateService.CoordinateToPosition("b8");
			pos2 = CoordinateService.CoordinateToPosition("c6");
			board = _boardService.UpdateBoard(board, ChessType.Color.Black, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/1PN5/P1PP1PPP/R1BQKBNR w KQkq - 1 4", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 
			pos1 = CoordinateService.CoordinateToPosition("c1");
			pos2 = CoordinateService.CoordinateToPosition("a3");
			board = _boardService.UpdateBoard(board, ChessType.Color.White, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("r1bqkbnr/p2ppppp/2n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR b KQkq - 2 4", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6
			pos1 = CoordinateService.CoordinateToPosition("c8");
			pos2 = CoordinateService.CoordinateToPosition("a6");
			board = _boardService.UpdateBoard(board, ChessType.Color.Black, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/R2QKBNR w KQkq - 3 5", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1
			pos1 = CoordinateService.CoordinateToPosition("a1");
			pos2 = CoordinateService.CoordinateToPosition("b1");
			board = _boardService.UpdateBoard(board, ChessType.Color.White, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR b Kkq - 4 5", board.FEN);
			//1. e4 c5 2. Nc3 b5 3. b3 Nc6 4. Ba3 Ba6 5. Rb1 Rb8
			pos1 = CoordinateService.CoordinateToPosition("a8");
			pos2 = CoordinateService.CoordinateToPosition("b8");
			board = _boardService.UpdateBoard(board, ChessType.Color.Black, pos1, pos2, string.Empty, new List<BoardHistory>());
			Assert.IsTrue(board.Matrix.Count == 32);
			Assert.AreEqual("1r1qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PP1PPP/1R1QKBNR w Kk - 5 6", board.FEN);
		}

		private void AssertStartingPosition(Dictionary<int, char> matrix) {
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

	}
}
