using System.Collections.Generic;

namespace Contracts.ViewModels {
	public class ChessBoardViewModel {
		public long GameId { get; set; }
		/// <summary>
		/// Notation that describes how we got to the FEN
		/// </summary>
		public string PGN { get; set; }
		/// <summary>
		/// Complete description of the game at the current moment
		/// </summary>
		public string FEN { get; set; }
		public bool IsCheck { get{ return IsWhiteCheck || IsBlackCheck; } }
		public bool IsWhiteCheck { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCheckmate { get; set; }
		/// <summary>
		/// Indicates whether the last move was successful
		/// </summary>
		public bool MoveSuccess { get; set; }
		/// <summary>
		/// Error message for failed move
		/// </summary>
		public string MoveFailureMessage { get; set; }
		/// <summary>
		/// The following properties are simply aspects of the FEN, but are very useful to carry around
		/// piece placement from white's perspective
		/// </summary>
		public string Position { get; set; }
		/// <summary>
		/// Contains all the position of all of the pieces on the board
		/// </summary>
		public Dictionary<int, char> Matrix { get; set; }
		/// <summary>
		/// contains all the attacks for every piece on the board
		/// "w" means white moves next, "b" means black.
		/// </summary>
		public char ActiveColor { get; set; }
		/// <summary>
		/// If neither side can castle, this is "-". Otherwise, this has one or more letters: "K" (White can castle kingside), "Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside).
		/// </summary>
		public string CastlingAvailability { get; set; }
		/// <summary>
		/// En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture.
		/// </summary>
		public string EnPassantTargetSquare { get; set; }
		/// <summary>
		/// This is the number of halfmoves since the last pawn advance or capture. This is used to determine if a draw can be claimed under the fifty-move rule.
		/// </summary>
		public int HalfmoveClock { get; set; }
		/// <summary>
		/// The number of the full move. It starts at 1, and is incremented after Black's move.
		/// </summary>
		public int FullmoveNumber { get; set; }
		public bool GameOver { get; set; }
		public char Winner { get; set; }
		/// <summary>
		/// The purpose of this property is for displaying that the opponent player offered a draw.
		/// A claim of a draw first counts as an offer of a draw, and the opponent may accept the draw without the arbiter examining the claim. Once a claim or draw offer has been made, it cannot be withdrawn. If the claim is verified or the draw offer accepted, the game is over. Otherwise, the offer or claim is nullified and the game continues; the draw offer is no longer in effect.
		/// b or w are the acceptable values
		/// </summary>
		public char DrawOffered { get; set; }
	}
}