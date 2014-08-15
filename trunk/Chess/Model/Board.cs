using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace Chess.Model {
	public struct Board {
		//notation that describes how we got to the FEN
		public string PGN { get; set; }
		//complete description of the game at the current moment
		public string FEN { get; set; }
		public bool IsCheck { get{ return IsWhiteCheck || IsBlackCheck; } }
		public bool IsWhiteCheck { get; set; }
		public bool IsBlackCheck { get; set; }
		public bool IsCheckmate { get; set; }
		public bool HasThreefoldRepition { get; set; }
		//indicates whether the last move was successful
		public bool MoveSuccess { get; set; }
		//Error message for failed move
		public string MoveFailureMessage { get; set; }
		//The following properties are simply aspects of the FEN, but are very useful to carry around
		//piece placement from white's perspective
		public string Position { get; set; }
		//contains all the position of all of the pieces on the board
		public Dictionary<int, char> Matrix { get; set; }
		//contains all the attacks for every piece on the board
		[XmlIgnore]
		[ScriptIgnore]
		public Dictionary<int, List<int>> AllAttacks { 
			get {
				var allAttacks = new Dictionary<int, List<int>>();
				foreach(var whiteAttack in WhiteAttacks) {
					allAttacks.Add(whiteAttack.Key, whiteAttack.Value);
				}
				foreach(var blackAttack in BlackAttacks) {
					allAttacks.Add(blackAttack.Key, blackAttack.Value);
				}
				return allAttacks;
			} 
		}
		[XmlIgnore]
		[ScriptIgnore]
		public Dictionary<int, List<int>> WhiteAttacks { get; set; }
		[XmlIgnore]
		[ScriptIgnore]
		public Dictionary<int, List<int>> BlackAttacks { get; set; }
		//"w" means white moves next, "b" means black.
		public char ActiveColor { get; set; }
		public ChessType.Color ActiveChessTypeColor {
			get{
				return this.ActiveColor == 'w' ? ChessType.Color.White : ChessType.Color.Black;
			}
		}
		//If neither side can castle, this is "-". Otherwise, this has one or more letters: "K" (White can castle kingside), "Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside).
		public string CastlingAvailability { get; set; }
		//En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture.
		public string EnPassantTargetSquare { get; set; }
		//This is the number of halfmoves since the last pawn advance or capture. This is used to determine if a draw can be claimed under the fifty-move rule.
		public int HalfmoveClock { get; set; }
		//The number of the full move. It starts at 1, and is incremented after Black's move.
		public int FullmoveNumber { get; set; }
	}
}