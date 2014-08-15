using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Chess.Model;

namespace Chess.ServiceLayer {
	// Pawn promotions are notated by appending an "=" to the destination square, followed by the piece the pawn is promoted to. 
	// "e8=Q". If the move is a checking move, the plus sign "+" is also appended; 
	// if the move is a checkmating move, the number sign "#" is appended instead. For example: "e8=Q#".
	// kingside castling is indicated by the sequence "O-O"; queenside castling is indicated by the sequence "O-O-O"
	public static class PGNService {
		public const char NullPiece = '-';
		public const char PawnPromotionIndicator = '=';

		public static Tuple<int, int> PGNMoveToSquarePair(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, ChessType.Color playerColor, string pgnMove) {
			int newPiecePosition = GetPositionFromPGNMove(pgnMove, playerColor);
			ChessType.PieceType piece = GetPieceTypeFromPGNMove(pgnMove);
			int piecePosition = GetCurrentPositionFromPGNMove(matrix, allAttacks, piece, playerColor, newPiecePosition, pgnMove);
			return Tuple.Create<int, int>(piecePosition, newPiecePosition);
		}
		public static string SquarePairToPGNMove(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, ChessType.Color playerColor, string startSquare, string endSquare, char promoteToPiece) {
			var pgnMove = "";
			try {
				pgnMove = squarePairToPGNMove(matrix, allAttacks, playerColor, startSquare, endSquare);
			} catch (Exception ex) {
				pgnMove = "Invalid";
			}
			if (promoteToPiece == PGNService.NullPiece) {
				return pgnMove;
			} else {
				var result = string.Concat(pgnMove, PGNService.PawnPromotionIndicator, promoteToPiece);
				return result;
			}
		}
		private static string squarePairToPGNMove(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, ChessType.Color playerColor, string startSquare, string endSquare) {
			var startPos = CoordinateService.CoordinateToPosition(startSquare);
			var endPos = CoordinateService.CoordinateToPosition(endSquare);
			var isCapture = matrix.ContainsKey(endPos);

			List<int> attacks;
			allAttacks.TryGetValue(startPos, out attacks);
			if (attacks != null && attacks.Any() && attacks.Contains(endPos)) {
				char piece = '0';
				matrix.TryGetValue(startPos, out piece);
				if (piece != '0') {
					if ((playerColor == ChessType.Color.White && char.IsUpper(piece)) || (playerColor == ChessType.Color.Black && char.IsLower(piece))) {
						char notationPiece = char.ToUpper(piece);
						var coord = CoordinateService.PositionToCoordinate(endPos);
						string pgnMove = getPgnMove(notationPiece, piece, coord, startPos, endPos, isCapture, matrix, allAttacks);
						return pgnMove;
					} else {
						throw new Exception("Color doesn't match given positions.");
					}
				}
			}
			throw new Exception("Bad coordinates were given.");
		}

		private static string getPgnMove(char notationPiece, char piece, string coord, int startPos, int endPos, bool isCapture, Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks) {
			string captureMarker = isCapture ? "x" : string.Empty;
			string pgnMove = getPGNMoveBeginState(notationPiece, coord, startPos, endPos, isCapture);
			string result = string.Empty;

			//figure out if additional information needs to be placed on the pgn move
			var otherSquaresOfThisTypeWithThisAttack = allAttacks.Where(a => a.Value.Contains(endPos)).Select(c => c.Key);
			var otherPiecesOfThisTypeWithThisAttack = matrix.Where(a => otherSquaresOfThisTypeWithThisAttack.Contains(a.Key) && a.Value == piece && a.Key != startPos);

			if (otherPiecesOfThisTypeWithThisAttack.Count() > 0) {
				var secondPiece = otherPiecesOfThisTypeWithThisAttack.First();

				if (char.ToUpper(secondPiece.Value) == 'P' && !isCapture) {
					result = string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
					return result;
				}

				//if other piece is on same the file of departure (if they differ); or
				var movingPieceFile = CoordinateService.PositionToFileChar(startPos);
				var otherPieceFile = CoordinateService.PositionToFileChar(secondPiece.Key);

				if (movingPieceFile != otherPieceFile) {
					if (notationPiece == 'P') {
						result = string.Concat(movingPieceFile, captureMarker, pgnMove);
					} else {
						result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
					}
					return result;
				} else {
					//the rank of departure (if the files are the same but the ranks differ)
					var movingPieceRank = CoordinateService.PositionToRankInt(startPos);
					var otherPieceRank = CoordinateService.PositionToRankInt(secondPiece.Key);
					if (movingPieceRank != otherPieceRank) {
						result = string.Concat(pgnMove.Substring(0, 1), movingPieceRank, captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
						return result;
					} else {
						//both the rank and file 
						//(if neither alone is sufficient to identify the piece—which occurs only in rare cases where one or more pawns have promoted, 
						//resulting in a player having three or more identical pieces able to reach the same square).
						result = string.Concat(pgnMove.Substring(0, 1), movingPieceFile, movingPieceRank, captureMarker, pgnMove.Substring(1, 2));
						return result;
					}
				}
			} else {
				result = string.Concat(pgnMove.Substring(0, 1), captureMarker, pgnMove.Substring(1, pgnMove.Length - 1));
				return result;
			}
		}
		private static string getPGNMoveBeginState(char notationPiece, string coord, int startPos, int endPos, bool isCapture) {
			string pgnMove = string.Empty;

			switch (notationPiece) {
				case 'P':
					if (isCapture) {
						var file = CoordinateService.PositionToFileChar(startPos);
						pgnMove = string.Concat(file, coord);
					}else{
						pgnMove = coord;
					}
					break;
				case 'K':
					var moveDiff = startPos - endPos;
					switch (moveDiff) {
						case -2:
							pgnMove = "O-O";
							break;
						case 2:
							pgnMove = "O-O-O";
							break;
						default:
							pgnMove = string.Concat(notationPiece, coord);
							break;
					}
					break;
				default:
					pgnMove = string.Concat(notationPiece, coord);
					break;
			}
			return pgnMove;
		}

		public static int GetCurrentPositionFromPGNMove(Dictionary<int, char> matrix, Dictionary<int, List<int>> allAttacks, ChessType.PieceType piece, ChessType.Color playerColor, int newPiecePosition, string pgnMove) {
			char pieceChar = GetPieceCharFromPieceTypeColor(piece, playerColor);
			var potentialSquares = allAttacks.Where(a => a.Value.Contains(newPiecePosition)).Select(c => c.Key);
			var potentialPositions = matrix.Where(a => potentialSquares.Contains(a.Key) && a.Value == pieceChar);
			if(!potentialPositions.Any()){
				return -1; //meaning no postion available
			}

			//x means capture and shouldn't be used in the equation below
			bool capture = isCapture(pgnMove);
			bool check = isCheck(pgnMove);
			bool castleKingside = isCastleKingside(pgnMove);
			bool castleQueenside = isCastleQueenside(pgnMove);
			bool isCastle = castleKingside || castleQueenside;
			var newPgnMove = pgnMove.Replace("x", "").Replace("+", "");

			if (isCastle) {
				return getOriginationPositionForCastling(playerColor);
			}
			
			if (potentialPositions.Count() < 1) {
				return potentialPositions.First().Key;
			}

			//todo: refactor to eliminate redundancy
			//look at the beginning of the pgnMove string to determine which of the pieces are the one that should be moved.
			//this should only happen if there are two pieces of the same type that can attack here.
			var moveLength = newPgnMove.Length;
			var ambiguityResolver = '-';
			switch (moveLength) {
				case 2:
					if (piece == ChessType.PieceType.Pawn) {
						if (!capture) { //todo: make sure this makes sense, i was distracted - seems ok two days later
							if (playerColor == ChessType.Color.White) {
								return potentialPositions.Where(a => a.Key == newPiecePosition - 8 || a.Key == newPiecePosition - 16).First().Key;
							} else {
								return potentialPositions.Where(a => a.Key == newPiecePosition + 8 || a.Key == newPiecePosition + 16).First().Key;
							}
						}
					}
					return -1; //indicates failure
				case 3: //this should be a pawn attack that can be made by two pawns
					ambiguityResolver = newPgnMove[0];
					var files = CoordinateService.GetEntireFile(CoordinateService.FileToInt(ambiguityResolver)); //this will always be a file if this is a pawn
					var pieces = potentialPositions.Where(a => files.Contains(a.Key)).ToList();
					if (pieces.Count() > 1) {
						throw new Exception("There should not be more than one item found here.");
					}
					return pieces.First().Key;
				case 4: //this would be any other piece
					ambiguityResolver = newPgnMove[1];
					var isRank = IsRank(ambiguityResolver); //this could be either a rank or a file
					List<int> ambiguityResolutionSet;
					if (isRank) {
						int rank = 0;
						Int32.TryParse(ambiguityResolver.ToString(), out rank);
						ambiguityResolutionSet = CoordinateService.GetEntireRank(rank - 1);//needs to be using zero-based rank offset
					} else {
						var iFile = CoordinateService.FileToInt(ambiguityResolver);
						ambiguityResolutionSet = CoordinateService.GetEntireFile(iFile);
					}
					var intersection = potentialPositions.Select(a => a.Key).Intersect(ambiguityResolutionSet);
					if (intersection.Count() > 1) {
						throw new Exception("There should not be more than one item found here.");
					}
					return intersection.First();
				case 5: //we have rank and file, so just find the piece. this should be very rare
					var _file = CoordinateService.FileToInt(newPgnMove[1]);
					int _rank = 0;
					Int32.TryParse(newPgnMove[2].ToString(), out _rank);
					int pos = CoordinateService.CoordinatePairToPosition(_file, _rank);
					return pos;
				default:
					return -1; //indicates failure
			}
		}
		
		public static bool IsRank(char potentialRank) {
			return char.IsNumber(potentialRank);
		}
		public static char GetPieceCharFromPieceTypeColor(ChessType.PieceType piece, ChessType.Color playerColor) {
			char pieceChar = 'a';
			switch (piece) {
				case ChessType.PieceType.Bishop:
					pieceChar = 'b';
					break;
				case ChessType.PieceType.Pawn:
					pieceChar = 'p';
					break;
				case ChessType.PieceType.King:
					pieceChar = 'k';
					break;
				case ChessType.PieceType.Knight:
					pieceChar = 'n';
					break;
				case ChessType.PieceType.Queen:
					pieceChar = 'q';
					break;
				case ChessType.PieceType.Rook:
					pieceChar = 'r';
					break;
			}
			if (playerColor == ChessType.Color.White) {
				pieceChar = char.ToUpper(pieceChar);
			}
			return pieceChar;
		}
		public static int GetPositionFromPGNMove(string pgnMove, ChessType.Color playerColor) {
			if (pgnMove == "O-O" || pgnMove == "O-O-O") {
				if(playerColor == ChessType.Color.White){
					if (pgnMove == "O-O"){
						return 6;
					} else {
						return 2;					
					}
				} else {
					if (pgnMove == "O-O") {
						return 62;
					} else {
						return 58;
					}				
				}
			}
			pgnMove = pgnMove.Replace("x", "").Replace("+", "").Replace("#", "");
			var result = pgnMove.Contains("=") ? CoordinateService.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 4, 2)) : CoordinateService.CoordinateToPosition(pgnMove.Substring(pgnMove.Length - 2, 2));
			return result;
		}
		public static ChessType.PieceType GetPieceTypeFromPGNMove(string pgnMove) {
			if (pgnMove.Length == 2) {
				return ChessType.PieceType.Pawn;
			}
			if (pgnMove == "O-O" || pgnMove == "O-O-O") {
				return ChessType.PieceType.King;
			}
			var piece = pgnMove[0]; //should not capitalize this to check because all piece disambiguity notation is caps, therefore a file indicator will not be.
			switch (piece) {
				case 'B':
				case 'b':
					return ChessType.PieceType.Bishop;
				case 'K':
				case 'k':
					return ChessType.PieceType.King;
				case 'N':
				case 'n':
					return ChessType.PieceType.Knight;
				case 'Q':
				case 'q':
					return ChessType.PieceType.Queen;
				case 'R':
				case 'r':
					return ChessType.PieceType.Rook;
			}
			return ChessType.PieceType.Pawn;
		}

		public static List<string> PGNSplit(string pgn) {
			if (string.IsNullOrEmpty(pgn)) { return null; }

			var regex = @"\d{1,3}\.";
			var splitResult = Regex.Split(pgn.Trim(), regex);
			return splitResult.ToList();
		}
		public static List<string> PGNSplit(string pgn, bool mostConsise) {
			if (string.IsNullOrEmpty(pgn)) { return null; }

			var pgnData = PGNSplit(pgn);
			if (!mostConsise) { return pgnData; }

			if (pgnData != null && pgnData.Any()) {
				var iterationData = pgnData.ToList();
				var emptyStuffs = iterationData.Where(a => a == " " || string.IsNullOrEmpty(a)).ToList();
				if (emptyStuffs != null && emptyStuffs.Any()) {
					foreach (var item in emptyStuffs) {
						iterationData.Remove(item);
					}
				}
				var returnValue = new List<string>();

				foreach (var item in iterationData) {
					var movePair = item.Trim().Split(' ');
					returnValue.Add(movePair[0]);
					if (movePair.Length > 1) {
						returnValue.Add(movePair[1]);
					}
				}
				return returnValue;
			}
			return null;
		}

		private static int getOriginationPositionForCastling(ChessType.Color color) {
			int rank = color == ChessType.Color.White ? 1 : 8;
			int file = 4;
			char fileChar = CoordinateService.IntToFile(file);
			var coord = string.Concat(fileChar, rank);
			int origination = CoordinateService.CoordinateToPosition(coord);
			return origination;
		}
		private static bool isCapture(string move) {
			bool retval = false;
			if (move.Contains('x')) {
				retval = true;
			}
			return retval;
		}
		private static bool isCheck(string move) {
			bool retval = false;
			if (move.Contains('+')) {
				retval = true;
			}
			return retval;
		}
		private static bool isCastleQueenside(string move) {
			bool retval = false;
			if (move.Contains("O-O-O")) {
				retval = true;
			}
			return retval;
		}
		private static bool isCastleKingside(string move) {
			bool retval = false;
			if (move.Contains("O-O")) {
				retval = true;
			}
			return retval;
		}
	}
}
