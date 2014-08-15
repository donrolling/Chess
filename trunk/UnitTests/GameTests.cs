using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using Chess.Model;
using Chess.ServiceLayer;

namespace UnitTests {
	[TestClass]
	public class GameTests {
		[TestMethod]
		public void StartGameTest() {
			Game game = new Game();

			game.Move(ChessType.Color.White, "e4");
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", game.Board.FEN);

			game.Move(ChessType.Color.Black, "e5");
			Assert.AreEqual("rnbqkbnr/pppp1ppp/8/4p3/4P3/8/PPPP1PPP/RNBQKBNR w KQkq e6 0 2", game.Board.FEN);
		}
		[TestMethod]
		public void MoveErrorTest() {
			Game game = new Game();

			game.Move(ChessType.Color.White, "e4");
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", game.Board.FEN);

			game.Move(ChessType.Color.White, "e5");
			Assert.IsNotNull(game.Board.MoveFailureMessage);
		}
		[TestMethod]
		public void InvalidMoveTest() {
			Game game = new Game();

			game.Move(ChessType.Color.White, "e4");
			Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", game.Board.FEN);

			game.Move(ChessType.Color.Black, "e7");
			Assert.IsNotNull(game.Board.MoveFailureMessage);
		}
		[TestMethod]
		public void CastleTest() {
			Game game = new Game("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/R3KBNR w KQkq - 3 5");

			game.Move(ChessType.Color.White, "Kc1");
			Assert.AreEqual("r2qkbnr/p2ppppp/b1n5/1pp5/4P3/BPN5/P1PPQPPP/2KR1BNR b kq - 4 5", game.Board.FEN);
		}
		[TestMethod]
		public void EnPassantTest() {
			Game game = new Game("r2qkbnr/p2pp1pp/b1n5/1pp1Pp2/8/BPN5/P1PPQPPP/R3KBNR w KQkq f6 0 6");
			game.Move(ChessType.Color.White, "exf6");
			Assert.AreEqual("r2qkbnr/p2pp1pp/b1n2P2/1pp5/8/BPN5/P1PPQPPP/R3KBNR b KQkq - 0 6", game.Board.FEN);
		}
		[TestMethod]
		public void ThreeFoldRepitionTest() {
			Game game = new Game("8/pp3p1k/2p2q1p/3r1P2/5R2/7P/P1PQ1P2/7K w - - 0 30"); //http://en.wikipedia.org/wiki/Threefold_repetition#Fischer_versus_Petrosian.2C_1971
			game.Move(ChessType.Color.White, "Qe2");
			game.Move(ChessType.Color.Black, "Qe5");
			game.Move(ChessType.Color.White, "Qh5");
			game.Move(ChessType.Color.Black, "Qf6");
			game.Move(ChessType.Color.White, "Qe2");
			game.Move(ChessType.Color.Black, "Re5");
			game.Move(ChessType.Color.White, "Qd3");
			game.Move(ChessType.Color.Black, "Rd5");
			game.Move(ChessType.Color.White, "Qe2");
			Assert.AreEqual(true, game.Board.HasThreefoldRepition);
		}
		[TestMethod]
		public void IsCheckTest() {
			Game game = new Game("8/pp3p1k/2p2P1p/3r4/4QR2/6qP/P1P2P2/7K b - - 0 34"); //http://en.wikipedia.org/wiki/Threefold_repetition#Fischer_versus_Petrosian.2C_1971
			Assert.IsTrue(game.Board.IsBlackCheck);

			//can check color move without resolving the check?
			game.Move(ChessType.Color.Black, "Rd1");
			Assert.IsTrue(!string.IsNullOrEmpty(game.Board.MoveFailureMessage));

			//make a proper move
			game.Move(ChessType.Color.Black, "Kh8");
			Assert.IsFalse(game.Board.IsBlackCheck);
			Assert.IsTrue(string.IsNullOrEmpty(game.Board.MoveFailureMessage));

			//now test white checking
			game = new Game("7k/pp3p2/2p2P1p/3r4/5R2/3Q3P/P1P2Pq1/7K w - - 0 36"); 
			Assert.IsTrue(game.Board.IsWhiteCheck);

			//can check color move without resolving the check?
			game.Move(ChessType.Color.White, "Qd5");
			Assert.IsTrue(!string.IsNullOrEmpty(game.Board.MoveFailureMessage));

			//make a proper move
			game.Move(ChessType.Color.White, "Kxg2");
			Assert.IsTrue(string.IsNullOrEmpty(game.Board.MoveFailureMessage));
		}
		[TestMethod]
		public void IsCheckmateTest1() {
			Game game = new Game("3Rr2k/pp3p2/2p2P1p/8/8/3Q1K1P/P1P2P2/8 w - - 0 40");
			game.Move(ChessType.Color.White, "Re8");
			Assert.IsTrue(game.Board.IsCheckmate);
			Assert.IsTrue(game.IsGameOver); //this won't happen unless there is a move, hence the move above
			Assert.IsTrue(game.Result == Game.GameResult.White);

			game = new Game("2Q5/pp3p1k/2p2P1p/8/5R2/6qP/P1Pr1P2/7K b - - 0 34");
			game.Move(ChessType.Color.Black, "Rd1");
			Assert.IsTrue(game.Board.IsCheckmate);
			Assert.IsTrue(game.IsGameOver); //this won't happen unless there is a move, hence the move above
			Assert.IsTrue(game.Result == Game.GameResult.Black);
		}
		[TestMethod]
		public void IsCheckmateTest2() {
			//test a checkmate where interposition is possible to save the king once before checkmate
			Game game = new Game("2Q5/pp3p1k/2p2P1p/8/4R3/6qP/P1Pr1P2/7K b - - 0 34");
			game.Move(ChessType.Color.Black, "Rd1");
			Assert.IsTrue(game.Board.IsWhiteCheck);
			Assert.IsFalse(game.Board.IsCheckmate);

			game.Move(ChessType.Color.White, "Re1");
			Assert.IsFalse(game.Board.IsWhiteCheck);

			game.Move(ChessType.Color.Black, "Re1");
			Assert.IsTrue(game.Board.IsCheckmate);
			Assert.IsTrue(game.IsGameOver); //this won't happen unless there is a move, hence the move above
			Assert.IsTrue(game.Result == Game.GameResult.Black);
		}
		[TestMethod]
		public void IsCheckmateTest3() {
			//test a checkmate where there are two attackers that mate and one may be interposed
			Game game = new Game("1k6/1p6/2p5/4R3/5B2/5K2/3r4/Q7 w - - 0 40");
			game.Move(ChessType.Color.White, "Re8");
			Assert.IsTrue(game.Board.IsBlackCheck);
			Assert.IsTrue(game.Board.IsCheckmate);
		}
		[TestMethod]
		public void IsCheckmateTest4() {
			//test a checkmate where king is put in check by the move that is made
			Game game = new Game("1k6/1p6/2p5/8/8/3rRK2/8/Q7 w - - 0 40");
			game.Move(ChessType.Color.White, "Re8");
			Assert.IsFalse(game.Board.MoveSuccess);
		}
		[TestMethod]
		public void IsCheckmateTest5() {
			Game game = new Game();
			var pgnGame = "1. e4 e6 2. Ke2 d5 3. Ke3 a5 4. Kd4 Nc6 5. Kd3 Nd4 6. Kxd4 b6 7. Bd3 Qf6 8. e5 Qf4 9. Kc3 Qxe5 10. Kb3 Ba6 11. Qe2 Qd4 12. c3 Qc5 13. Kc2 b5 14. Na3 b4 15. Bxa6 bxa3 16. b3 Qe7 17. Bb7 Rd8 18. d4 Qf6 19. Qb5 Ke7 20. Qxa5 Qf5 21. Kd1 Qg4 22. f3 Nf6 23. Qxc7 Nd7 24. Bc6 Qe4 25. fxe4 Kf6 26. Nf3 Bd6 27. Qxd6 g6 28. b4 h5 29. b5 h4 30. b6 g5 31. b7 g4 32. b8=q Nxb8 33. Qe5 Kg6 34. Ng5 h3 35. g3 Rc8 36. Rf1 Nxc6 37. Qf6 Kh5 38. Nxf7 Rhf8 39. Qxe6 Rxf7 40. Rxf7 dxe4 41. Rf5";
			var pgnData = PGNService.PGNSplit(pgnGame, true);
			var count = pgnData.Count;
			if (pgnData != null && pgnData.Any()) {
				for (int i = 0; i < count - 1; i++){
					var pgn = pgnData[i];
					if (pgn.Contains('.')) {
						continue;
					}
					game.Move(game.Board.ActiveChessTypeColor, pgn);			 
				}
			}
			//last move before checkmate
			game.Move(game.Board.ActiveChessTypeColor, pgnData[count - 1]);
			Assert.IsTrue(game.IsGameOver);
			Assert.AreEqual(Game.GameResult.White, game.Result);
		}		
	}
}
