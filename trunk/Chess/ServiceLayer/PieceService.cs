using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Model;

namespace Chess.ServiceLayer {
	public static class PieceService {
		public static Tuple<ChessType.PieceType, ChessType.Color> GetChessTypeFromChar(char piece){
			var pieceType = CoordinateService.GetPieceTypeFromChar(piece);
			var pieceColor = CoordinateService.GetColorFromChar(piece);
			return Tuple.Create<ChessType.PieceType, ChessType.Color>(pieceType, pieceColor);
		}				
		public static List<int> GetPieceAttacks(string fen, KeyValuePair<int, char> square, bool ignoreKing = false) {
			var position = square.Key;
			var piece = square.Value;

			var result = PieceService.GetChessTypeFromChar(piece);
			var pieceType = result.Item1;
			var pieceColor = result.Item2;

			var matrix = NotationService.CreateMatrixFromFEN(fen);

			switch (pieceType) {
				case ChessType.PieceType.Invalid:
					return new List<int>();
				case ChessType.PieceType.Pawn:
					var enPassantPosition = CoordinateService.CoordinateToPosition(fen.Split(' ')[3]);
					return GetPawnAttacks(matrix, position, pieceColor, enPassantPosition);
				case ChessType.PieceType.Knight:
					return GetKnightAttacks(matrix, position, pieceColor);
				case ChessType.PieceType.Bishop:
					return GetBishopAttacks(matrix, position, pieceColor, ignoreKing);
				case ChessType.PieceType.Rook:
					return GetRookAttacks(matrix, position, pieceColor, ignoreKing);
				case ChessType.PieceType.Queen:
					return GetQueenAttacks(matrix, position, pieceColor, ignoreKing);
				case ChessType.PieceType.King:
					var castleAvailability = fen.Split(' ')[2];
					return GetKingAttacks(fen, position, pieceColor, castleAvailability);
			}

			return new List<int>();
		}

		public static List<int> GetKingAttacks(string fen, int position, ChessType.Color pieceColor, string castleAvailability) {
			List<int> attacks = new List<int>();
			var matrix = NotationService.CreateMatrixFromFEN(fen);

			var positionList = new List<int> { -9, -8, -7, -1, 1, 7, 8, 9 };

			if ( //make sure castle is available
				(pieceColor == ChessType.Color.White && (castleAvailability.Contains("K") || castleAvailability.Contains("Q")))
				|| (pieceColor == ChessType.Color.Black && (castleAvailability.Contains("k") || castleAvailability.Contains("q")))
			) {
				positionList.Add(2);
				positionList.Add(-2);
			}

			if (position % 8 == 0) {
				positionList.Remove(-1);
				positionList.Remove(-9);
				positionList.Remove(7);
			}
			if (position % 8 == 7) {
				positionList.Remove(1);
				positionList.Remove(9);
				positionList.Remove(-7);
			}

			foreach (var positionShim in positionList) {
				var tempPos = position + positionShim;
				if (CoordinateService.IsValidCoordinate(tempPos)) {
					if (isValidMove(matrix, tempPos, pieceColor)) {
						var isCastle = Math.Abs(positionShim) == 2; //are we trying to move two squares? if so, this is a castle attempt
						if (!isCastle) {
							attacks.Add(tempPos);
						} else {
							var direction = positionShim > 0 ? 1 : -1;
							int clearPathPos = tempPos;
							int clearPathFile = 4;
							do { //make sure the path is clear
								clearPathPos += direction;
								clearPathFile = CoordinateService.PositionToFile(clearPathPos);
							} while (isValidMove(matrix, clearPathPos, pieceColor) && clearPathFile > 0 && clearPathFile < 8);
							if (
								(pieceColor == ChessType.Color.White && (positionShim == -2 && clearPathPos == 0) || (positionShim == 2 && clearPathPos == 7))
								|| (pieceColor == ChessType.Color.Black && (positionShim == -2 && clearPathPos == 56) || (positionShim == 2 && clearPathPos == 63))
							) {
								if (matrix.ContainsKey(clearPathPos)) {
									var edgePiece = matrix[clearPathPos];
									if (
										(pieceColor == ChessType.Color.White && edgePiece == 'R')
										|| (pieceColor == ChessType.Color.Black && edgePiece == 'r')
									) {
										attacks.Add(tempPos);
									}
								}
							}
						}
					}
				}
			}

			attacks = removeKingChecksFromKingMoves(fen, attacks, pieceColor, matrix);

			return attacks;
		}
		public static List<int> GetQueenAttacks(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetOrthogonals(matrix, position, pieceColor, ignoreKing));
			attacks.AddRange(CoordinateService.GetDiagonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetRookAttacks(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetOrthogonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetBishopAttacks(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, bool ignoreKing) {
			List<int> attacks = new List<int>();
			attacks.AddRange(CoordinateService.GetDiagonals(matrix, position, pieceColor, ignoreKing));
			return attacks;
		}
		public static List<int> GetKnightAttacks(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor) {
			List<int> attacks = new List<int>();
			
			var coord = CoordinateService.PositionToCoordinate(position);
			var file = CoordinateService.FileToInt(coord[0]);
			var rank = (int)coord[1];

			var positions = new List<int>{ 6, 10, 15, 17, -6, -10, -15, -17 };
			foreach (var pos in positions) {
				var tempPosition = position + pos;
				bool isValid = isValidKnightMove(position, tempPosition, file, rank);
				bool isGoodMove = isValidMove(matrix, tempPosition, pieceColor);
				if (isValid && isGoodMove) {
					attacks.Add(tempPosition);
				}
			}

			attacks = removeInvalidPieces(matrix, attacks, pieceColor);
			return attacks;
		}
		public static List<int> GetPawnAttacks(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor, int enPassantPosition) {
			List<int> attacks = new List<int>();
			var coord = CoordinateService.PositionToCoordinate(position);
			int file = CoordinateService.FileToInt(coord[0]);
			int rank = CoordinateService.PositionToRankInt(position);

			var directionIndicator = pieceColor == ChessType.Color.White ? 1 : -1;
			var rankIndicator = pieceColor == ChessType.Color.White ? 2 : 7;

			var nextRank = (rank + directionIndicator);
			attacks.Add(CoordinateService.CoordinatePairToPosition(file, nextRank));
			
			if (file - 1 >= 0) {
				//get attack square on left
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				if (isValidPawnAttack(matrix, leftPos, pieceColor)) {
					attacks.Add(leftPos);
				}
			}
			if (file + 1 <= 7) {
				//get attack square on right
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (isValidPawnAttack(matrix, rightPos, pieceColor)) {
					attacks.Add(rightPos);
				}
			}
			//have to plus one here because rank is zero based and coordinate is base 1
			if ((rank + 1) == rankIndicator) {
				attacks.Add(CoordinateService.CoordinatePairToPosition(file, nextRank + directionIndicator));
			}

			//add en passant position
			if(enPassantPosition > -1){
				var leftPos = CoordinateService.CoordinatePairToPosition(file - 1, nextRank);
				var rightPos = CoordinateService.CoordinatePairToPosition(file + 1, nextRank);
				if (enPassantPosition == leftPos || enPassantPosition == rightPos) {
					attacks.Add(enPassantPosition);
				}
			}

			return attacks;		
		}

		public static Dictionary<int, List<int>> GetAttacks(ChessType.Color color, string fen, bool ignoreKing = false) {
			Dictionary<int, List<int>> allAttacks = new Dictionary<int, List<int>>();
			var matrix = NotationService.CreateMatrixFromFEN(fen);
			IEnumerable<KeyValuePair<int, char>> queriedPieces = getMatrixOfOneColor(color, matrix, ignoreKing);
			foreach(var square in queriedPieces) {
				var list = PieceService.GetPieceAttacks(fen, square, ignoreKing);
				allAttacks.Add(square.Key, list);
			}
			return allAttacks;
		}

		private static IEnumerable<KeyValuePair<int, char>> getMatrixOfOneColor(ChessType.Color color, Dictionary<int, char> matrix, bool ignoreKing = false) {
			IEnumerable<KeyValuePair<int, char>> queriedPieces = new Dictionary<int, char>();
			if (color == ChessType.Color.White) {
				queriedPieces = matrix.Where(a => char.IsUpper(a.Value));
			} else {
				queriedPieces = matrix.Where(a => char.IsLower(a.Value));
			}
			if(ignoreKing) {
				return queriedPieces.Where(a => a.Value != 'k' && a.Value != 'K');
			} else {
				return queriedPieces;			
			}
		}

		private static ChessType.Color getColor(Dictionary<int, char> matrix, int square) {
			var piece = matrix.Where(a => a.Key == square).FirstOrDefault();
			return char.IsUpper(piece.Value) ? ChessType.Color.White : ChessType.Color.Black;
		}

		private static List<int> removeKingChecksFromKingMoves(string fen, List<int> kingAttacks, ChessType.Color pieceColor, Dictionary<int, char> matrix) {
			ChessType.Color oppositePieceColor = CoordinateService.GetOppositeColor(pieceColor);
			var pieceAttacks = GetAttacks(oppositePieceColor, fen, true);

			var conflictingAttacks =	from p in pieceAttacks.SelectMany(a => a.Value)
										join k in kingAttacks on p equals k
										select p;
			
			foreach (var conflictingAttack in conflictingAttacks) {
				bool removeAttack = true;
				var pieces = pieceAttacks.Where(a => a.Value.Contains(conflictingAttack));
				if(pieces != null){
					if (pieces.Count() == 1){ //if there are more, it's not possible that we'd need to keep the attack
						var key = pieces.First().Key;
						var piece = matrix[key];
						//this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
						if (char.ToUpper(piece) == 'P') {
							var directionIndicator = pieceColor == ChessType.Color.White ? -1 : 1; //make this backwards of normal
							var onSameFile = key + (directionIndicator * 8) == conflictingAttack ? true : false;
							if (onSameFile) {
								removeAttack = false;
							}
						}
					}
				}
				if (removeAttack) {
					kingAttacks.Remove(conflictingAttack);
				}
			}

			return kingAttacks;
		}
		private static bool determineCheck(Dictionary<int, char> matrix, List<int> proposedAttacks, ChessType.Color pieceColor) {
			var king = CoordinateService.FindPiece(matrix, ChessType.PieceType.King, pieceColor);
			if (king != null && king.Any() && proposedAttacks.Contains(king.First())) {
				return true;
			}
			return false;
		}
		private static bool isValidKnightMove(int position, int tempPosition, int file, int rank) {
			var tempCoord = CoordinateService.PositionToCoordinate(tempPosition);
			var tempFile = CoordinateService.FileToInt(tempCoord[0]);
			var tempRank = (int)tempCoord[1];

			var fileDiff = Math.Abs(tempFile - file);
			var rankDiff = Math.Abs(tempRank - rank);

			if (fileDiff > 2 || fileDiff < 1) {
				return false;
			}
			if (rankDiff > 2 || rankDiff < 1) {
				return false;
			}

			return true;
		}
		private static List<int> removeInvalidPieces(Dictionary<int, char> matrix, List<int> attacks, ChessType.Color pieceColor) {
			attacks = attacks.Where(a => CoordinateService.IsValidCoordinate(a)).ToList();

			var ownPieces = from a in attacks
							join m in matrix on a equals m.Key
							where removeOwnPieces(pieceColor, m.Value)
							select a;

			attacks = attacks.Except(ownPieces).ToList();
			return attacks;
		}
		private static bool isValidPawnAttack(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor) {
			if(CoordinateService.IsValidCoordinate(position)) {
				return true;
				//if (matrix.Select(a => a.Key).Contains(position)) {
					//var blockingPiece = matrix.Where(a => a.Key == position).First();
					//if (CoordinateService.CanAttackPiece(pieceColor, blockingPiece.Value)) {
					//	return true;
					//}
				//}
			}
			return false;
		}
		private static bool isValidMove(Dictionary<int, char> matrix, int position, ChessType.Color pieceColor) {
			if (CoordinateService.IsValidCoordinate(position)) {
				if (matrix.Select(a => a.Key).Contains(position)) {
					var blockingPiece = matrix[position];
					if (CoordinateService.CanAttackPiece(pieceColor, blockingPiece)) {
						return true;
					}else{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private static bool removeOwnPieces(ChessType.Color pieceColor, char piece) {
			var result = pieceColor == ChessType.Color.White ? char.IsUpper(piece) : char.IsLower(piece);
			return result;
		}
	}
}