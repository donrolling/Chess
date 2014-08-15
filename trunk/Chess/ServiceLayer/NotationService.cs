using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Model;

namespace Chess.ServiceLayer {
	public static class NotationService {
		private const string defaultCastlingAvailability = "KQkq";
		private const char nullPiece = '\0';

		public static Dictionary<int, char> UpdateMatrix(Dictionary<int, char> matrix, int piecePosition, int newPiecePosition) {
			Dictionary<int, char> matrixCopy = new Dictionary<int, char>(matrix.Count, matrix.Comparer);
			foreach (KeyValuePair<int, char> entry in matrix) {
				matrixCopy.Add(entry.Key, (char)entry.Value);
			}

			var piece = matrixCopy[piecePosition];
			matrixCopy.Remove(piecePosition);
			matrixCopy.Remove(newPiecePosition);
			matrixCopy.Add(newPiecePosition, piece);
			return matrixCopy;
		}
		/// <summary>
		/// Updates the matrix for pawn promotion
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="newPiecePosition"></param>
		/// <param name="pieceColor"></param>
		/// <param name="piecePromotedTo"></param>
		/// <returns></returns>
		public static Dictionary<int, char> UpdateMatrix(Dictionary<int, char> matrix, int newPiecePosition, ChessType.Color pieceColor, char piecePromotedTo) {
			Dictionary<int, char> matrixCopy = new Dictionary<int, char>(matrix.Count, matrix.Comparer);
			foreach (KeyValuePair<int, char> entry in matrix) {
				matrixCopy.Add(entry.Key, (char)entry.Value);
			}

			var piece = pieceColor == ChessType.Color.White ? char.ToUpper(piecePromotedTo) : char.ToLower(piecePromotedTo);
			matrixCopy.Remove(newPiecePosition);
			matrixCopy.Add(newPiecePosition, piece);
			return matrixCopy;
		}
		public static Dictionary<int, char> CreateMatrixFromFEN(string fen) {
			var matrix = new Dictionary<int, char>();
			var fenPosition = fen.Split(' ');
			var rows = fenPosition[0].Split('/');

			for (int i = 0; i < 8; i++) {
				int rowIndex = 7 - i;
				int leftSideIndex = 8 * (rowIndex);

				int charIndex = 0;
				string row = rows[i];
				foreach (char c in row) {
					if (char.IsNumber(c)) {
						int number = 0;
						Int32.TryParse(c.ToString(), out number);
						charIndex = charIndex + number;
					} else {
						int index = leftSideIndex + charIndex;
						matrix.Add(index, c);
						charIndex++;
					}
				}
			}

			return matrix;
		}
		public static string CreateNewFENFromBoard(Board board, Dictionary<int, char> matrix, int piecePosition, int newPiecePosition) {
			//"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
			string position = createNewPositionFromMatrix(matrix);
			string castlingAvailability = getCastlingAvailability(matrix, board.CastlingAvailability, piecePosition, newPiecePosition);
			string enPassantCoord = getEnPassantCoord(matrix, board.ActiveColor, piecePosition, newPiecePosition);
			string halfmoveClock = gethalfmoveClock(board.Matrix, board.HalfmoveClock, piecePosition, newPiecePosition);
			string fullmoveNumber = getFullmoveNumber(board.FullmoveNumber, board.ActiveColor);

			var fenParams = new string[6] { position, CoordinateService.GetOppositeColor(board.ActiveColor).ToString(), castlingAvailability, enPassantCoord, halfmoveClock, fullmoveNumber };
			string fen = string.Join(" ", fenParams);
			return fen;
		}

		private static string getCastlingAvailability(Dictionary<int, char> matrix, string castlingAvailability, int piecePosition, int newPiecePosition) {
			char movingPiece = nullPiece;
			matrix.TryGetValue(newPiecePosition, out movingPiece);

			if (movingPiece == 'r' || movingPiece == 'R' || movingPiece == 'k' || movingPiece == 'K') {
				switch (piecePosition) {
					case 0: //R
						return castlingAvailability.Replace("Q", "");
					case 7: //R
						return castlingAvailability.Replace("K", "");
					case 56: //r
						return castlingAvailability.Replace("q", "");
					case 63: //r
						return castlingAvailability.Replace("k", "");
					case 4:  //K
						var retval = castlingAvailability.Replace("K", "").Replace("Q", "");
						return retval;
					case 60: //k
						var result = castlingAvailability.Replace("k", "").Replace("q", "");
						return result;
				}
			}

			if (string.IsNullOrEmpty(castlingAvailability)) {
				return "-";
			} else {
				return castlingAvailability;
			}
		}
		/// <summary>
		/// Get the halfmove clock.
		/// </summary>
		/// <param name="matrix">Must be the current matrix, not the new one.</param>
		/// <param name="halfmoveClock">Current halfmove clock.</param>
		/// <param name="piecePosition">Moving piece position.</param>
		/// <param name="newPiecePosition">Capture piece position.</param>
		/// <returns></returns>
		private static string gethalfmoveClock(Dictionary<int, char> matrix, int halfmoveClock, int piecePosition, int newPiecePosition) {
			char movingPiece = nullPiece;
			matrix.TryGetValue(piecePosition, out movingPiece);

			char capturePiece = nullPiece;
			matrix.TryGetValue(newPiecePosition, out capturePiece);

			//if we're captuing, or moving a pawn the clock resets
			if (capturePiece != nullPiece || (movingPiece == 'p' || movingPiece == 'P')) {
				return "0";
			}
			return (halfmoveClock + 1).ToString();
		}
		private static string getFullmoveNumber(int fullmoveNumber, char activeColor) {
			if (activeColor == 'b') {
				return (fullmoveNumber + 1).ToString();
			}
			return (fullmoveNumber).ToString();
		}
		private static string getEnPassantCoord(Dictionary<int, char> matrix, char activeColor, int piecePosition, int newPiecePosition) {
			var piece = matrix[newPiecePosition];
			if (piece == 'p' || piece == 'P') {
				var diff = Math.Abs(piecePosition - newPiecePosition);
				if(diff == 16){
					var moveMarker = 8;
					if (activeColor == 'w') { moveMarker = (moveMarker * -1); }
					var enPassantSquare = newPiecePosition + moveMarker;
					var enPassantCoord = CoordinateService.PositionToCoordinate(enPassantSquare);
					return enPassantCoord;
				}
			}
			return "-";
		}
		private static string createNewPositionFromMatrix(Dictionary<int, char> matrix) {
			StringBuilder position = new StringBuilder();
			for (int i = 0; i < 8; i++) {
				int leftSideIndex = 8 * (7 - i);
				var row = matrix.Where(a => a.Key >= leftSideIndex && a.Key < leftSideIndex + 8);
				if (row != null && row.Any()) {
					int missingPieceCount = 0;
					for (int j = 0; j < 8; j++) {
						var square = leftSideIndex + j;
						char piece = nullPiece;
						matrix.TryGetValue(square, out piece);
						if (piece != nullPiece) {
							if (missingPieceCount > 0) {
								position.Append(missingPieceCount.ToString());
								missingPieceCount = 0;
							}
							position.Append(piece);
						} else {
							missingPieceCount += 1;
						}
					}
					if (missingPieceCount > 0) {
						position.Append(missingPieceCount.ToString());
					}
				} else {
					position.Append('8');
				}
				if (i < 7) {
					position.Append('/');
				}
			}
			return position.ToString();
		}
	}
}
