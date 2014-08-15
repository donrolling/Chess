using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.ServiceLayer;
using Chess.Model;
using Chess;

namespace UnitTests {
	[TestClass]
	public class PGNTests {
		private static BoardService _boardService;

		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			_boardService = new BoardService();
		}
		
		[TestMethod]
		public void Basic() {
			//Assert.AreEqual(16, PGNService.GetPositionFromPGNMove("Na3"));
			//Assert.AreEqual(8, PGNService.GetPositionFromPGNMove("Na2"));
			Assert.AreEqual(ChessType.PieceType.King, PGNService.GetPieceTypeFromPGNMove("Kb2"));
			Assert.AreEqual(ChessType.PieceType.Queen, PGNService.GetPieceTypeFromPGNMove("Qa2"));
			Assert.AreEqual(ChessType.PieceType.Rook, PGNService.GetPieceTypeFromPGNMove("Ra2"));
			Assert.AreEqual(ChessType.PieceType.Bishop, PGNService.GetPieceTypeFromPGNMove("Bb2"));
			Assert.AreEqual(ChessType.PieceType.Knight, PGNService.GetPieceTypeFromPGNMove("Na2"));
			Assert.AreEqual(ChessType.PieceType.Pawn, PGNService.GetPieceTypeFromPGNMove("a2"));

			Assert.AreEqual(ChessType.PieceType.King, PGNService.GetPieceTypeFromPGNMove("kb2"));
			Assert.AreEqual(ChessType.PieceType.Queen, PGNService.GetPieceTypeFromPGNMove("qa2"));
			Assert.AreEqual(ChessType.PieceType.Rook, PGNService.GetPieceTypeFromPGNMove("ra2"));
			Assert.AreEqual(ChessType.PieceType.Bishop, PGNService.GetPieceTypeFromPGNMove("bb2"));
			Assert.AreEqual(ChessType.PieceType.Knight, PGNService.GetPieceTypeFromPGNMove("na2"));
			Assert.AreEqual(ChessType.PieceType.Pawn, PGNService.GetPieceTypeFromPGNMove("a2"));
		}

		[TestMethod]
		public void AllAttacks() {
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);

			var positions = PGNService.PGNMoveToSquarePair(board.Matrix, board.AllAttacks, ChessType.Color.White, "e4");
			Assert.AreEqual(12, positions.Item1);
			Assert.AreEqual(28, positions.Item2);

			var positions2 = PGNService.PGNMoveToSquarePair(board.Matrix, board.AllAttacks, ChessType.Color.White, "e3");
			Assert.AreEqual(12, positions2.Item1);
			Assert.AreEqual(20, positions2.Item2);
		}

		[TestMethod]
		public void PGNMoveDifferentiation(){
			var fen = "4r3/N2R3p/1k2BPp1/8/8/2P5/PP3PPP/3R2K1 w - - 0 30";
			var game = new Game(fen);
			var pos = PGNService.GetCurrentPositionFromPGNMove(game.Board.Matrix, game.Board.AllAttacks, ChessType.PieceType.Rook, ChessType.Color.White, 43, "R1d6");
			Assert.AreEqual(3, pos);
		}
	}
}
