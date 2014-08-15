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
	public class AttackTests {
		private static BoardService _boardService;

		[ClassInitialize]
		public static void ClassSetUp(TestContext testContext) {
			_boardService = new BoardService();
		}

		[TestMethod]
		public void PawnAttack() {
			//white queenside rook pawn, opening moves
			var a2PawnAttacks = PieceService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(8, 'P'));
			Assert.IsTrue(a2PawnAttacks.Contains(16));
			Assert.IsTrue(a2PawnAttacks.Contains(17));
			Assert.IsTrue(a2PawnAttacks.Contains(24));
			Assert.IsTrue(a2PawnAttacks.Count() == 3);

			//white king pawn, opening moves
			var e2PawnAttacks = PieceService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(12, 'P'));
			Assert.IsTrue(e2PawnAttacks.Contains(19));
			Assert.IsTrue(e2PawnAttacks.Contains(20));
			Assert.IsTrue(e2PawnAttacks.Contains(21));
			Assert.IsTrue(e2PawnAttacks.Contains(28));
			Assert.IsTrue(e2PawnAttacks.Count() == 4);

			//black
			var d7PawnAttacks = PieceService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(51, 'p'));
			Assert.IsTrue(d7PawnAttacks.Contains(42));
			Assert.IsTrue(d7PawnAttacks.Contains(43));
			Assert.IsTrue(d7PawnAttacks.Contains(44));
			Assert.IsTrue(d7PawnAttacks.Contains(35));
			Assert.IsTrue(d7PawnAttacks.Count() == 4);


			Assert.IsTrue(CoordinateService.IsDiagonalMove(0, 63));
			Assert.IsFalse(CoordinateService.IsDiagonalMove(0, 62));

			Assert.IsTrue(CoordinateService.IsDiagonalMove(7, 56));
			Assert.IsFalse(CoordinateService.IsDiagonalMove(7, 55));

			Assert.IsTrue(CoordinateService.IsDiagonalMove(33, 60));
			Assert.IsFalse(CoordinateService.IsDiagonalMove(29, 37));

			//test some errors that I found involving pawns moving diagonally when they're not capturing
			Game game = new Game("1k3r2/7p/4BPp1/1N1R4/8/2P5/PP3PPP/R5K1 w  - 3 26");
			game.Move(ChessType.Color.White, "e3");
			Assert.IsFalse(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void PawnAttack2() {
			string fen = "3r3r/1P1n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R w - - 0 32";
			Game game = new Game(fen);
			game.Move(ChessType.Color.White, "b8=q");
			Assert.IsTrue(game.Board.MoveSuccess);
			Assert.AreEqual("1Q1r3r/3n1p2/2BQpk2/3p4/3PP1pp/p1P2N2/P5PP/R1BK3R b - - 0 32", game.Board.FEN);
		}

		[TestMethod]
		public void RankFileTests() {
			var a1RookAttackRank = CoordinateService.GetEntireRank(0);
			Assert.IsTrue(a1RookAttackRank.Contains(0));
			Assert.IsTrue(a1RookAttackRank.Contains(1));
			Assert.IsTrue(a1RookAttackRank.Contains(2));
			Assert.IsTrue(a1RookAttackRank.Contains(3));
			Assert.IsTrue(a1RookAttackRank.Contains(4));
			Assert.IsTrue(a1RookAttackRank.Contains(5));
			Assert.IsTrue(a1RookAttackRank.Contains(6));
			Assert.IsTrue(a1RookAttackRank.Contains(7));
			var a1RookAttackFile = CoordinateService.GetEntireFile(0);
			Assert.IsTrue(a1RookAttackFile.Contains(0));
			Assert.IsTrue(a1RookAttackFile.Contains(8));
			Assert.IsTrue(a1RookAttackFile.Contains(16));
			Assert.IsTrue(a1RookAttackFile.Contains(24));
			Assert.IsTrue(a1RookAttackFile.Contains(32));
			Assert.IsTrue(a1RookAttackFile.Contains(40));
			Assert.IsTrue(a1RookAttackFile.Contains(48));
			Assert.IsTrue(a1RookAttackFile.Contains(56));			
		}

		[TestMethod]
		public void RookAttack() {
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);

			var a1RookAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(0, 'R'));
			//Assert.IsTrue(a1RookAttacks.Contains(1));
			//Assert.IsTrue(a1RookAttacks.Contains(2));
			//Assert.IsTrue(a1RookAttacks.Contains(3));
			//Assert.IsTrue(a1RookAttacks.Contains(4));
			//Assert.IsTrue(a1RookAttacks.Contains(5));
			//Assert.IsTrue(a1RookAttacks.Contains(6));
			//Assert.IsTrue(a1RookAttacks.Contains(7));
			//Assert.IsTrue(a1RookAttacks.Contains(8));
			//Assert.IsTrue(a1RookAttacks.Contains(16));
			//Assert.IsTrue(a1RookAttacks.Contains(24));
			//Assert.IsTrue(a1RookAttacks.Contains(32));
			//Assert.IsTrue(a1RookAttacks.Contains(40));
			//Assert.IsTrue(a1RookAttacks.Contains(48));
			//Assert.IsTrue(a1RookAttacks.Contains(56));
			Assert.IsTrue(a1RookAttacks.Count() == 0);

			var a8RookAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(56, 'r'));
			//Assert.IsTrue(a8RookAttacks.Contains(0));
			//Assert.IsTrue(a8RookAttacks.Contains(8));
			//Assert.IsTrue(a8RookAttacks.Contains(16));
			//Assert.IsTrue(a8RookAttacks.Contains(24));
			//Assert.IsTrue(a8RookAttacks.Contains(32));
			//Assert.IsTrue(a8RookAttacks.Contains(40));
			//Assert.IsTrue(a8RookAttacks.Contains(48));
			//Assert.IsTrue(a8RookAttacks.Contains(57));
			//Assert.IsTrue(a8RookAttacks.Contains(58));
			//Assert.IsTrue(a8RookAttacks.Contains(59));
			//Assert.IsTrue(a8RookAttacks.Contains(60));
			//Assert.IsTrue(a8RookAttacks.Contains(61));
			//Assert.IsTrue(a8RookAttacks.Contains(62));
			//Assert.IsTrue(a8RookAttacks.Contains(63));
			Assert.IsTrue(a8RookAttacks.Count() == 0);

			var h1RookAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(7, 'R'));
			//Assert.IsTrue(h1RookAttacks.Contains(0));
			//Assert.IsTrue(h1RookAttacks.Contains(1));
			//Assert.IsTrue(h1RookAttacks.Contains(2));
			//Assert.IsTrue(h1RookAttacks.Contains(3));
			//Assert.IsTrue(h1RookAttacks.Contains(4));
			//Assert.IsTrue(h1RookAttacks.Contains(5));
			//Assert.IsTrue(h1RookAttacks.Contains(6));
			//Assert.IsTrue(h1RookAttacks.Contains(15));
			//Assert.IsTrue(h1RookAttacks.Contains(23));
			//Assert.IsTrue(h1RookAttacks.Contains(31));
			//Assert.IsTrue(h1RookAttacks.Contains(39));
			//Assert.IsTrue(h1RookAttacks.Contains(47));
			//Assert.IsTrue(h1RookAttacks.Contains(55));
			//Assert.IsTrue(h1RookAttacks.Contains(63));
			Assert.IsTrue(h1RookAttacks.Count() == 0);

			var h8RookAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(63, 'r'));
			//Assert.IsTrue(h8RookAttacks.Contains(7));
			//Assert.IsTrue(h8RookAttacks.Contains(15));
			//Assert.IsTrue(h8RookAttacks.Contains(23));
			//Assert.IsTrue(h8RookAttacks.Contains(31));
			//Assert.IsTrue(h8RookAttacks.Contains(39));
			//Assert.IsTrue(h8RookAttacks.Contains(47));
			//Assert.IsTrue(h8RookAttacks.Contains(55));
			//Assert.IsTrue(h8RookAttacks.Contains(56));
			//Assert.IsTrue(h8RookAttacks.Contains(57));
			//Assert.IsTrue(h8RookAttacks.Contains(58));
			//Assert.IsTrue(h8RookAttacks.Contains(59));
			//Assert.IsTrue(h8RookAttacks.Contains(60));
			//Assert.IsTrue(h8RookAttacks.Contains(61));
			//Assert.IsTrue(h8RookAttacks.Contains(62));
			Assert.IsTrue(h8RookAttacks.Count() == 0);
		}

		[TestMethod]
		public void BishopAttack() {
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);

			var whiteBishopAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(2, 'B'));
			//Assert.IsTrue(whiteBishopAttacks.Contains(9));
			//Assert.IsTrue(whiteBishopAttacks.Contains(16));
			//Assert.IsTrue(whiteBishopAttacks.Contains(11));
			//Assert.IsTrue(whiteBishopAttacks.Contains(20));
			//Assert.IsTrue(whiteBishopAttacks.Contains(29));
			//Assert.IsTrue(whiteBishopAttacks.Contains(38));
			//Assert.IsTrue(whiteBishopAttacks.Contains(47));
			Assert.IsTrue(whiteBishopAttacks.Count() == 0);

			var whiteBishopAttacks2 = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(27, 'B'));
			Assert.IsTrue(whiteBishopAttacks2.Contains(20));
			Assert.IsTrue(whiteBishopAttacks2.Contains(34));
			Assert.IsTrue(whiteBishopAttacks2.Contains(41));
			Assert.IsTrue(whiteBishopAttacks2.Contains(48));
			Assert.IsTrue(whiteBishopAttacks2.Contains(18));
			Assert.IsTrue(whiteBishopAttacks2.Contains(36));
			Assert.IsTrue(whiteBishopAttacks2.Contains(45));
			Assert.IsTrue(whiteBishopAttacks2.Contains(54));
			Assert.IsTrue(whiteBishopAttacks2.Count() == 8);
		}

		[TestMethod]
		public void QueenAttack() {
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);

			var whiteQueenAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(3, 'Q'));
			Assert.IsTrue(whiteQueenAttacks.Count() == 0);

			var whiteQueenAttacks2 = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(27, 'Q'));
			Assert.IsTrue(whiteQueenAttacks2.Contains(18));
			Assert.IsTrue(whiteQueenAttacks2.Contains(20));

			Assert.IsTrue(whiteQueenAttacks2.Contains(34));
			Assert.IsTrue(whiteQueenAttacks2.Contains(36));

			Assert.IsTrue(whiteQueenAttacks2.Contains(41));
			Assert.IsTrue(whiteQueenAttacks2.Contains(45));

			Assert.IsTrue(whiteQueenAttacks2.Contains(19));
			Assert.IsTrue(whiteQueenAttacks2.Contains(35));
			Assert.IsTrue(whiteQueenAttacks2.Contains(43));

			Assert.IsTrue(whiteQueenAttacks2.Contains(24));
			Assert.IsTrue(whiteQueenAttacks2.Contains(25));
			Assert.IsTrue(whiteQueenAttacks2.Contains(26));
			Assert.IsTrue(whiteQueenAttacks2.Contains(28));
			Assert.IsTrue(whiteQueenAttacks2.Contains(29));
			Assert.IsTrue(whiteQueenAttacks2.Contains(30));
			Assert.IsTrue(whiteQueenAttacks2.Contains(31));

			Assert.IsTrue(whiteQueenAttacks2.Contains(48));
			Assert.IsTrue(whiteQueenAttacks2.Contains(51));
			Assert.IsTrue(whiteQueenAttacks2.Contains(54));

			Assert.IsTrue(whiteQueenAttacks2.Count() == 19);
		}

		[TestMethod]
		public void KnightAttack() {
			Board board = _boardService.SetStartPosition(FEN.StartingPosition);

			var knightAttacks = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(35, 'N'));
			Assert.IsTrue(knightAttacks.Contains(18));
			Assert.IsTrue(knightAttacks.Contains(20));
			Assert.IsTrue(knightAttacks.Contains(25));
			Assert.IsTrue(knightAttacks.Contains(29));
			Assert.IsTrue(knightAttacks.Contains(41));
			Assert.IsTrue(knightAttacks.Contains(45));
			Assert.IsTrue(knightAttacks.Contains(50));
			Assert.IsTrue(knightAttacks.Contains(52));
			Assert.IsTrue(knightAttacks.Count() == 8);

			var knightAttacks2 = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(1, 'N'));
			Assert.IsTrue(knightAttacks2.Contains(18));
			Assert.IsTrue(knightAttacks2.Contains(16));
			Assert.IsTrue(knightAttacks2.Count() == 2);

			var knightAttacks3 = PieceService.GetPieceAttacks(board.FEN, new KeyValuePair<int, char>(25, 'N'));
			Assert.IsTrue(knightAttacks3.Contains(19));
			Assert.IsTrue(knightAttacks3.Contains(35));
			Assert.IsTrue(knightAttacks3.Contains(40));
			Assert.IsTrue(knightAttacks3.Contains(42));
			Assert.IsTrue(knightAttacks3.Count() == 4);
		}

		[TestMethod]
		public void KingAttack() {
			var kingAttacks = PieceService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(4, 'K'));
			Assert.AreEqual(0, kingAttacks.Count());

			//artifically placing the white king on square 44 to see his attacks
			var kingAttacks2 = PieceService.GetPieceAttacks(FEN.StartingPosition, new KeyValuePair<int, char>(44, 'K'));
			Assert.AreEqual(3, kingAttacks2.Count());

			//test white king move availability from g1
			var kingAttacks3 = PieceService.GetPieceAttacks("3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 w - - 0 1", new KeyValuePair<int, char>(6, 'K'));
			Assert.AreEqual(2, kingAttacks3.Count());

			//it actually should be white's move here, but not for this test
			//move queen down to checkmate the king and move sure that the king has no attacks
			var kingAttacks4 = PieceService.GetPieceAttacks("3q1rk1/5pbp/5Qp1/8/8/2B5/5PPP/6K1 b - - 0 1", new KeyValuePair<int, char>(6, 'K'));
			Assert.IsTrue(kingAttacks4.Contains(5));
			Assert.IsTrue(kingAttacks4.Contains(7));
			Assert.AreEqual(2, kingAttacks4.Count());

			var kingAttacks5 = PieceService.GetPieceAttacks("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 3 5", new KeyValuePair<int, char>(4, 'K'));
			Assert.IsTrue(kingAttacks5.Contains(2));
			Assert.IsTrue(kingAttacks5.Contains(3));
			Assert.AreEqual(2, kingAttacks5.Count());
		}

		[TestMethod]
		public void KingAttacks2() {
			//king can't move in front of a pawn, this is wrong, let's fix it
			//var kingAttacks6 = PieceService.GetPieceAttacks("rnbqkbnr/1pp2ppp/4p3/p2p4/4P3/4K3/PPPP1PPP/RNBQ1BNR w kq a6 0 4", new KeyValuePair<int, char>(20, 'K'));
			//Assert.AreEqual(5, kingAttacks6.Count());
			string fen = "rnbqkbnr/1pp2ppp/4p3/p2p4/4P3/4K3/PPPP1PPP/RNBQ1BNR w kq a6 0 4";
			Game game = new Game(fen);
			game.Move(ChessType.Color.White, "Kd4");
			Assert.IsTrue(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks3() {
			var positions = CoordinateService.GetKingPositionsDuringCastle(60, 63);
			Assert.AreEqual(2, positions.Count());
			Assert.AreEqual(61, positions[0]);
			Assert.AreEqual(62, positions[1]);

			var positions2 = CoordinateService.GetKingPositionsDuringCastle(60, 56);
			Assert.AreEqual(2, positions2.Count());
			Assert.AreEqual(59, positions2[0]);
			Assert.AreEqual(58, positions2[1]);

			var positions3 = CoordinateService.GetKingPositionsDuringCastle(4, 7);
			Assert.AreEqual(2, positions3.Count());
			Assert.AreEqual(5, positions3[0]);
			Assert.AreEqual(6, positions3[1]);

			var positions4 = CoordinateService.GetKingPositionsDuringCastle(4, 0);
			Assert.AreEqual(2, positions4.Count());
			Assert.AreEqual(3, positions4[0]);
			Assert.AreEqual(2, positions4[1]);
		}

		[TestMethod]
		public void KingAttacks4() {
			string fen = "r1bqk2r/ppp2ppp/8/3P4/1Q2P3/6P1/PPP2P1P/R1B1KB1R b KQkq - 0 9";
			Game game = new Game(fen);

			bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(game.Board.Matrix, game.Board.FEN, game.Board.ActiveChessTypeColor, 60, 63);
			Assert.IsTrue(isCastleThroughCheck);

			game.Move(ChessType.Color.Black, "Kg8");
			Assert.IsFalse(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks5() {
			string fen = "r3k2r/pp3ppp/2p2q2/3P3b/1Q2PB2/1P6/P1P1BPPP/R3K2R w KQkq - 3 13";
			Game game = new Game(fen);

			bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(game.Board.Matrix, game.Board.FEN, game.Board.ActiveChessTypeColor, 4, 0);
			Assert.IsFalse(isCastleThroughCheck);

			game.Move(ChessType.Color.White, "Kc1");
			Assert.IsTrue(game.Board.MoveSuccess);
		}

		[TestMethod]
		public void KingAttacks6() {
			string fen = "2r5/8/2n1Q3/5R1k/3Pp1p1/p1P3Pp/P6P/R1BK4 b - - 1 41";
			//Game game = new Game(fen);
			//game.Move(ChessType.Color.White, "Rf5");
			//Assert.IsTrue(game.Board.MoveSuccess);

			var kingAttacks2 = PieceService.GetPieceAttacks(fen, new KeyValuePair<int, char>(39, 'k'));
			Assert.AreEqual(0, kingAttacks2.Count());
		}
	}
}
