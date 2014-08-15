using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Model;

namespace Chess.ServiceLayer {
	public static class CoordinateService {
		public const string Files = "abcdefgh";
		public static List<ChessType.DiagonalDirection> DiagonalLines = new List<ChessType.DiagonalDirection> { ChessType.DiagonalDirection.UpLeft, ChessType.DiagonalDirection.UpRight, ChessType.DiagonalDirection.DownLeft, ChessType.DiagonalDirection.DownRight };
		public static List<ChessType.Direction> OrthogonalLines = new List<ChessType.Direction> { ChessType.Direction.RowUp, ChessType.Direction.RowDown, ChessType.Direction.FileUp, ChessType.Direction.FileDown };

		public static int CoordinatePairToPosition(int file, int rank) {
			var fileChar = IntToFile(file);
			var coord = fileChar + (rank + 1).ToString();
			return CoordinateToPosition(coord);
		}
		public static string PositionToCoordinate(int position) {
			var file = PositionToFileChar(position);
			var rank = (PositionToRankInt(position) + 1).ToString();
			return string.Concat(file, rank);
		}
		public static int CoordinateToPosition(string coordinate) {
			if(coordinate == "-"){ return -1; }

			var coord = coordinate.Substring(coordinate.Length - 2, 2);
			var file = FileToInt(coord[0]);

			int rank = 0;
			Int32.TryParse(coord[1].ToString(), out rank);

			int position = file + (8 * (rank - 1));
			return position;
		}

		public static char GetOppositeColor(char activeColor) {
			return activeColor == 'w' ? 'b' : 'w';
		}
		public static ChessType.Color GetOppositeColor(ChessType.Color pieceColor) {
			return pieceColor == ChessType.Color.White ? ChessType.Color.Black : ChessType.Color.White;
		}

		public static int FileToInt(char file) {
			return (int)(file - 97);
		}
		public static char IntToFile(int file) {
			return (char)(file + 97);
		}

		public static int AbsDiff(int piecePos, int newPiecePos) {
			return Math.Abs(piecePos - newPiecePos);
		}

		public static char PositionToFileChar(int position) {
			var file = (char)((position % 8) + 97);
			return file;
		}
		public static int PositionToFileInt(int position) {
			var file = (position % 8);
			return file;
		}
		public static int PositionToFile(int position) {
			var file = (position % 8);
			return file;
		}
		public static int PositionToRankInt(int position) {
			var rank = (int)(position / 8);
			return rank;
		}

		public static List<int> GetEntireFile(int file) {
			List<int> attacks = new List<int>();

			var ind = file % 8;
			attacks.Add(ind);
			for (int i = 1; i < 8; i++) {
				attacks.Add((i * 8) + ind);
			}

			return attacks;
		}
		public static List<int> GetEntireRank(int rank) {
			List<int> attacks = new List<int>();

			var ind = (rank % 8) * 8;
			attacks.Add(ind);
			for (int i = 1; i < 8; i++) {
				attacks.Add(ind + i);
			}

			return attacks;
		}

		public static List<int> GetDiagonals(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, bool ignoreKing = false) {
			List<int> attacks = new List<int>();
			foreach(var direction in DiagonalLines) {
				attacks.AddRange(GetDiagonalLine(matrix, position, direction, pieceColor, ignoreKing));
			}
			return attacks;
		}
		public static List<int> GetDiagonalLine(Dictionary<int, char> matrix, int position, ChessType.DiagonalDirection direction, ChessType.Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			int diagonalLine = getIteratorByDirectionEnum(direction);
			int square = position;
			do {
				if(CanDoDiagonalsFromStartPosition(position, diagonalLine)) {
					square = square + diagonalLine;
					if(!IsValidCoordinate(square)) {
						break;
					}
					var matrixContainsKey = matrix.ContainsKey(square);
					if(matrixContainsKey) {
						var blockingPiece = matrix[square];
						if(CanAttackPiece(pieceColor, blockingPiece)) {
							attacks.Add(square);
						}
						bool breakAfterAction = BreakAfterAction(ignoreKing, blockingPiece, pieceColor);
						if(breakAfterAction) {
							break;
						}
					} else {
						attacks.Add(square);
					}
				}
			} while(IsValidDiagonalCoordinate(square));
			return attacks;
		}

		public static List<int> GetOrthogonals(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, bool ignoreKing = false) {
			List<int> attacks = new List<int>();
			foreach(var orthogonalLine in OrthogonalLines) {
				var line = GetOrthogonalLine(matrix, position, orthogonalLine, pieceColor, ignoreKing);
				if(line != null && line.Any()) {
					attacks.AddRange(line);
				}
			}
			return attacks;
		}
		public static List<int> GetOrthogonalLine(Dictionary<int, char> matrix, int position, ChessType.Direction direction, ChessType.Color pieceColor, bool ignoreKing) {
			int endCondition = getEndCondition(direction, position);

			List<int> attacks = new List<int>();
			var iterator = getIteratorByDirectionEnum(direction);
			for(var square = position + iterator; square != endCondition + iterator; square = square + iterator) {
				if(!IsValidCoordinate(square)) { break; }

				var matrixContainsKey = matrix.ContainsKey(square);
				if(matrixContainsKey) {
					var blockingPiece = matrix[square];
					if(CanAttackPiece(pieceColor, blockingPiece)) {
						attacks.Add(square);
					}
					bool breakAfterAction = BreakAfterAction(ignoreKing, blockingPiece, pieceColor);
					if(breakAfterAction) {
						break;
					}
				} else {
					attacks.Add(square);
				}
			}
			return attacks;
		}

		public static bool IsValidCoordinate(int position) {
			return position >= 0 && position <= 63;
		}
		public static bool CanAttackPiece(ChessType.Color pieceColor, char attackedPiece) {
			if (pieceColor == ChessType.Color.White && char.IsLower(attackedPiece)) {
				return true;
			}
			if (pieceColor == ChessType.Color.Black && char.IsUpper(attackedPiece)) {
				return true;
			}
			return false;		
		}

		public static bool IsDiagonalMove(int startPosition, int endPosition) {
			var startMod = startPosition % 8;
			var endMod = endPosition % 8;
			var modDiff = Math.Abs(startMod - endMod);

			var startRow = CoordinateService.PositionToRankInt(startPosition);
			var endRow = CoordinateService.PositionToRankInt(endPosition);
			var rowDiff = Math.Abs(startRow - endRow);
			if (modDiff == rowDiff) {
				return true;
			}
			return false;
		}
		private static bool CanDoDiagonalsFromStartPosition(int startPosition, int direction) {
			bool isLeftSide = startPosition % 8 == 0;
			bool isRightSide = startPosition % 8 == 7;

			if (isLeftSide && (direction == 7 || direction == -9)) { return false; }
			if (isRightSide && (direction == -7 || direction == 9)) { return false; }
			return true;
		}
		private static bool IsValidDiagonalCoordinate(int position) {
			if (!IsValidCoordinate(position)) { return false; }
			if (position % 8 == 0 || position % 8 == 7) { return false; }
			if (position < 7 || position > 56) { return false; }
			return true;
		}
		private static int getIteratorByDirectionEnum(ChessType.Direction direction){
			switch(direction) {
				case ChessType.Direction.RowUp:
					return 8;
				case ChessType.Direction.RowDown:
					return -8;
				case ChessType.Direction.FileUp:
					return 1;
				case ChessType.Direction.FileDown:
					return -1;
			}
			return 0;
		}
		private static int getIteratorByDirectionEnum(ChessType.DiagonalDirection direction) {
			switch(direction) {
				case ChessType.DiagonalDirection.UpLeft:
					return 7;
				case ChessType.DiagonalDirection.UpRight:
					return 9;
				case ChessType.DiagonalDirection.DownLeft:
					return -9;
				case ChessType.DiagonalDirection.DownRight:
					return -7;
			}
			return 0;
		}
		private static int getEndCondition(ChessType.Direction direction, int position) {
			int file = CoordinateService.PositionToFileInt(position);
			int rank = CoordinateService.PositionToRankInt(position);

			switch(direction) {
				case ChessType.Direction.RowUp:
					return CoordinateService.GetEntireFile(file).Max();
				case ChessType.Direction.RowDown:
					return CoordinateService.GetEntireFile(file).Min();
				case ChessType.Direction.FileUp:
					return CoordinateService.GetEntireRank(rank).Max();
				case ChessType.Direction.FileDown:
					return CoordinateService.GetEntireRank(rank).Min();
			}

			return 0;
		}

		public static bool BreakAfterAction(bool ignoreKing, char blockingPiece, ChessType.Color pieceColor) {
			//if ignoreKing is true, then we won't break after we hit the king 
			//because we're trying to determine if the king will be in check if he moves to one of these squares.
			bool breakAfterAction = false;
			if (ignoreKing) {
				bool isOpposingKing = IsOpposingKing(blockingPiece, pieceColor);
				if (!isOpposingKing) {
					breakAfterAction = true;
				}
			} else {
				breakAfterAction = true;
			}
			return breakAfterAction;
		}
		/// <summary>
		/// Determines if the char passed in is the king for the color opposite of the color passed in.
		/// </summary>
		/// <param name="piece">The piece that might be your opponent's king.</param>
		/// <param name="pieceColor">The color of the current player.</param>
		/// <returns></returns>
		public static bool IsOpposingKing(char piece, ChessType.Color pieceColor) {
			return pieceColor == ChessType.Color.White ? (piece == 'k' ? true : false) : (piece == 'K' ? true : false);
		}

		public static List<int> FindPiece(Dictionary<int, char> matrix, ChessType.PieceType pieceType, ChessType.Color color) {
			var pieceChar = GetCharFromPieceType(pieceType, color);
			var square = matrix.Where(a => a.Value == pieceChar).Select(a => a.Key).ToList();
			return square;
		}
		public static char GetCharFromPieceType(ChessType.PieceType pieceType, ChessType.Color color) {
			switch (pieceType) {
				case ChessType.PieceType.King:
					return color == ChessType.Color.White ? 'K' : 'k';
				case ChessType.PieceType.Queen:
					return color == ChessType.Color.White ? 'Q' : 'q';
				case ChessType.PieceType.Bishop:
					return color == ChessType.Color.White ? 'B' : 'b';
				case ChessType.PieceType.Knight:
					return color == ChessType.Color.White ? 'N' : 'n';
				case ChessType.PieceType.Rook:
					return color == ChessType.Color.White ? 'R' : 'r';
				case ChessType.PieceType.Pawn:
					return color == ChessType.Color.White ? 'P' : 'p';
			}
			return 'I';
		}
		public static ChessType.PieceType GetPieceTypeFromChar(char piece) {
			char indicator = piece.ToString().ToUpper()[0];
			switch (indicator) {
				case 'K':
					return ChessType.PieceType.King;
				case 'Q':
					return ChessType.PieceType.Queen;
				case 'B':
					return ChessType.PieceType.Bishop;
				case 'N':
					return ChessType.PieceType.Knight;
				case 'R':
					return ChessType.PieceType.Rook;
				case 'P':
					return ChessType.PieceType.Pawn;
			}
			return ChessType.PieceType.Invalid;
		}
		public static ChessType.Color GetColorFromChar(char piece) {
			if (char.IsLower(piece)) {
				return ChessType.Color.Black;
			}
			return ChessType.Color.White;
		}

		public static bool DetermineCastleThroughCheck(Dictionary<int, char> matrix, string fen, ChessType.Color color, int kingPos, int rookPos) {
			var oppositeColor = color == ChessType.Color.White ? ChessType.Color.Black : ChessType.Color.White;
			var enemyAttacks = PieceService.GetAttacks(oppositeColor, fen).SelectMany(a => a.Value);
			var positions = CoordinateService.GetKingPositionsDuringCastle(kingPos, rookPos);
			var arePositionsAttacked = (from e in enemyAttacks
										join p in positions on e equals p
										select p).Any();
			return arePositionsAttacked;
		}
		public static int[] GetKingPositionsDuringCastle(int kingPos, int rookPos) {
			int direction = kingPos < rookPos ? 1 : -1;
			int[] result = new int[2];
			for (int i = 0; i < 2; i++) {
				result[i] = kingPos + (direction * (i + 1));
			}
			return result;
		}
	}
}
