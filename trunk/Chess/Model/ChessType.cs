using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Model {
	public class ChessType {
		public enum PieceType {
			Invalid,
			Pawn,
			Knight,
			Bishop,
			Rook,
			Queen,
			King
		}

		public enum Color {
			Invalid,
			Black,
			White
		}

		public enum Direction {
			Invalid,
			RowUp,
			RowDown,
			FileUp,
			FileDown
		}

		public enum DiagonalDirection {
			Invalid,
			UpLeft,
			UpRight,
			DownLeft,
			DownRight
		}
	}
}
