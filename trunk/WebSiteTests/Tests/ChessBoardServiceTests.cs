using System;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contracts.Model;
using Service.Services;

namespace WebSiteTests {
	[TestClass]
	public class ChessBoardServiceTests {
		[TestMethod]
		public void RestoreGameTest() {
			var fen = "1k3r2/7p/4BPp1/1N1R4/8/2P5/PP3PPP/R5K1 w  - 3 26";
			var pgn = "1. e4 e5 2. d4 d4 3. Qd4 Bb4 4. c3 Bd6 5. e5 Be7 6. Nf3 Nc6 7. Qg4 g6 8. Bg5 Bg5 9. Ng5 Qg5 10. Qg5 Nce7 11. Qe7 Ke7 12. Bc4 Nf6 13. f6 Kd8 14. Bxf7 d5 15. O-O Rg8 16. Rd1 c5 17. Rxd5 Bd7 18. Bxg8 Kc7 19. Na3 b5 20. Rxc5 Kb6 21. Rd5 Be6 22. Bxe6 Rf8 23. Nxb5 Kc6 24. Nxa7 Kc7 25. Nb5 Kb8";
			GameData GameData = new GameData(){
				FEN = fen,
				PGN = pgn,
				Id = 3013
			};
			Game game = ChessBoardService.GetGameFromGameData(GameData);
			Assert.AreEqual(fen, game.Board.FEN);
		}
	}
}
