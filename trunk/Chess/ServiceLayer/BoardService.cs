using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Model;

namespace Chess.ServiceLayer {
	public class BoardService {
		public Board SetStartPosition(string fen) {
			return getNewBoard(fen, string.Empty, false, string.Empty);
		}
		//Positions should be numbered 0-63 where a1 is 0
		public Board UpdateBoard(Board board, ChessType.Color color, int piecePosition, int newPiecePosition, string pgnMove, List<BoardHistory> boardHistory) {
			Dictionary<int, char> matrix = NotationService.UpdateMatrix(board.Matrix, piecePosition, newPiecePosition);
			var piece = matrix[newPiecePosition];
			
			bool isCastle = this.isCastle(piece, piecePosition, newPiecePosition);
			if(isCastle) { //if is castle, update matrix again
				if (board.IsCheck) {
					var errorMessage = "Can't castle out of check.";
					var invalidBoard = getNewBoard(board.FEN, board.PGN, board.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidBoard;					
				}

				var rookPosition = getRookPositionsForCastle(color, piecePosition, newPiecePosition);
				bool isCastleThroughCheck = CoordinateService.DetermineCastleThroughCheck(matrix, board.FEN, color, piecePosition, rookPosition.Item1);
				if (!isCastleThroughCheck) {
					matrix = NotationService.UpdateMatrix(matrix, rookPosition.Item1, rookPosition.Item2);
				} else {
					var errorMessage = "Can't castle through check.";
					var invalidBoard = getNewBoard(board.FEN, board.PGN, board.HasThreefoldRepition, string.Empty, errorMessage);
					return invalidBoard;
				}
			}

			bool isEnPassant = this.isEnPassant(piece, piecePosition, newPiecePosition, board.EnPassantTargetSquare);
			if(isEnPassant) { //if is en passant, update matrix again
				var pawnPassing = color == ChessType.Color.White ? (newPiecePosition - 8) : (newPiecePosition + 8);
				matrix.Remove(pawnPassing);
			}

			bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
			if (isPawnPromotion) { //if is a pawn promotion, update matrix again
				var piecePromotedTo = pgnMove.Substring(pgnMove.IndexOf(PGNService.PawnPromotionIndicator) + 1, 1)[0];
				matrix = NotationService.UpdateMatrix(matrix, newPiecePosition, color, piecePromotedTo);
			}

			var validMove = isValidMove(board, color, piece, piecePosition, newPiecePosition, isEnPassant);
			if (!validMove) {
				var errorMessage = "Invalid move.";
				var invalidBoard = getNewBoard(board.FEN, board.PGN, board.HasThreefoldRepition, string.Empty, errorMessage);
				return invalidBoard;
			}

			var newFEN = NotationService.CreateNewFENFromBoard(board, matrix, piecePosition, newPiecePosition);
			var hasThreefoldRepition = this.hasThreefoldRepition(color, board, boardHistory, newFEN); 
			var newBoard = getNewBoard(newFEN, board.PGN, hasThreefoldRepition, pgnMove);

			var putsOwnKingInCheck = (board.ActiveChessTypeColor == ChessType.Color.White && newBoard.IsWhiteCheck) || (board.ActiveChessTypeColor == ChessType.Color.Black && newBoard.IsBlackCheck);
			if (putsOwnKingInCheck) {
				var checkedOwnKingBoard = getNewBoard(board.FEN, board.PGN, board.HasThreefoldRepition, string.Empty, "You must move out of check, or at the very least, not move into check.");
				return checkedOwnKingBoard;
			}

			return newBoard;
		}

		public Board UpdateBoardWithError(Board board, string errorMessage) {
			var newBoard = getNewBoard(board.FEN, board.PGN, board.HasThreefoldRepition, string.Empty, errorMessage);
			return newBoard;
		}
		public BoardHistory GetBoardHistory(Board board) {
			var boardHistory = new BoardHistory(){
				Position = board.Position,
				CastlingAvailability = board.CastlingAvailability,
				EnPassantTargetSquare = board.EnPassantTargetSquare
			};
			return boardHistory;
		}

		private Board getNewBoard(string fen, string pgn, bool hasThreefoldRepition, string pgnMove, string errorMessage = null) {
			Board board = new Board();

			bool isResign = false;
			bool isDraw = false;

			board.FEN = fen;
			board.HasThreefoldRepition = hasThreefoldRepition;
			var gameData = board.FEN.Split(' ');
			board.Position = gameData[0];
			board.ActiveColor = gameData[1][0];
			board.CastlingAvailability = gameData[2];
			board.EnPassantTargetSquare = gameData[3];

			int halfmoveClock = 0;
			int fullmoveNumber = 0;
			Int32.TryParse(gameData[4], out halfmoveClock);
			Int32.TryParse(gameData[5], out fullmoveNumber);
			board.HalfmoveClock = halfmoveClock;
			board.FullmoveNumber = fullmoveNumber;

			board.Matrix = NotationService.CreateMatrixFromFEN(board.FEN);
			board.WhiteAttacks = PieceService.GetAttacks(ChessType.Color.White, fen);
			board.BlackAttacks = PieceService.GetAttacks(ChessType.Color.Black, fen);

			var whiteKingSquare = board.Matrix.Where(a => a.Value == 'K').Single().Key;
			var blackKingSquare = board.Matrix.Where(a => a.Value == 'k').Single().Key;

			var attacksThatCheckWhite = board.BlackAttacks.Where(a => a.Value.Contains(whiteKingSquare));
			var attacksThatCheckBlack = board.WhiteAttacks.Where(a => a.Value.Contains(blackKingSquare));

			bool isCheck = false;
			board.IsWhiteCheck = isRealCheck(board.Matrix, attacksThatCheckWhite, board.ActiveChessTypeColor, whiteKingSquare);
			board.IsBlackCheck = isRealCheck(board.Matrix, attacksThatCheckBlack, board.ActiveChessTypeColor, blackKingSquare);
			
			if (!string.IsNullOrEmpty(pgnMove)) {
				bool isPawnPromotion = pgnMove.Contains(PGNService.PawnPromotionIndicator);
				if(isPawnPromotion && isCheck){
					pgnMove = string.Concat(pgnMove, '#');
				}
				var pgnNumbering = (board.ActiveColor == 'b' ? board.FullmoveNumber.ToString() + ". " : string.Empty);
				var nextPGNMove = string.Concat(pgnNumbering, pgnMove, ' ');
				board.PGN = pgn + nextPGNMove;
			}else{ board.PGN = pgn; }

			if(board.IsCheck) {
				var checkedKing = board.ActiveChessTypeColor == ChessType.Color.White ? whiteKingSquare : blackKingSquare; //trust me this is right
				var checkedColor = board.ActiveChessTypeColor == ChessType.Color.White ? ChessType.Color.White : ChessType.Color.Black; //trust me this is right
				board.IsCheckmate = isCheckmate(checkedColor, board.Matrix, checkedKing, board.WhiteAttacks, board.BlackAttacks);
				if(board.IsCheckmate){
					var score = string.Concat(" ", board.ActiveChessTypeColor == ChessType.Color.White ? "1-0" : "0-1");
					board.PGN += score;
				}
			} else {
				if (isDraw || isResign) {
					if (isDraw) {
						var score = string.Concat(" ", "1/2-1/2");
						board.PGN += score;					
					}
					if (isResign) {
						var score = string.Concat(" ", board.ActiveChessTypeColor == ChessType.Color.White ? "1-0" : "0-1");
						board.PGN += score;					
					}
				}
			}

			if(!string.IsNullOrEmpty(errorMessage)) {
				board.MoveFailureMessage = errorMessage;
				board.MoveSuccess = false;
			} else {
				board.MoveSuccess = true;
			}

			return board;
		}

		private bool isRealCheck(Dictionary<int, char> matrix, IEnumerable<KeyValuePair<int, List<int>>> attacksThatCheck, ChessType.Color activeChessTypeColor, int kingSquare) {
			bool isRealCheck = false;
			if (attacksThatCheck != null && attacksThatCheck.Any()) {
				isRealCheck = true;
				if (attacksThatCheck.Count() == 1) { //if there are more, it's not possible that we'd need to remove the attack
					var key = attacksThatCheck.First().Key;
					var attackingPiece = matrix[key];
					//this code is here to remove the possibility that the king is said to be in check by an enemy pawn when he is directly in front of the pawn
					if (char.ToUpper(attackingPiece) == 'P') {
						var onSameFile = (key % 8) == (kingSquare % 8) ? true : false;
						if (onSameFile) {
							isRealCheck = false;
						}
					}
				}
			}
			return isRealCheck;
		}
		private bool isValidMove(Board board, ChessType.Color color, char piece, int piecePosition, int newPiecePosition, bool isEnPassant) {
			if(char.ToUpper(piece) == 'P'){
				var isDiagonalMove = CoordinateService.IsDiagonalMove(piecePosition, newPiecePosition);
				if (isDiagonalMove) {
					var isCapture = board.Matrix.ContainsKey(newPiecePosition);
					if (isCapture || isEnPassant) {
						return true;
					}else{
						return false;
					}
				}
			}
			return true;
		}
		private bool isEnPassant(char piece, int piecePosition, int newPiecePosition, string enPassantTargetSquare) {
			if(char.ToUpper(piece) != 'P'){ return false; } //only pawns can perform en passant
			var enPassantPosition = CoordinateService.CoordinateToPosition(enPassantTargetSquare);
			if (enPassantPosition != newPiecePosition) { return false; } //if we're not moving to the en passant position, this is not en passant
			var moveDistance = Math.Abs(piecePosition - newPiecePosition);
			if (!new List<int> { 7, 9 }.Contains(moveDistance)) { return false; } //is this a diagonal move?
			if (char.IsLower(piece) && piecePosition < newPiecePosition) { return false; } //black can't move up
			if (char.IsUpper(piece) && piecePosition > newPiecePosition) { return false; } //black can't move down
			return true;
		}
		private bool isCastle(char piece, int piecePosition, int newPiecePosition) {
			bool isCastle = false;
			if (piece == 'K' || piece == 'k') {
				var distance = Math.Abs(piecePosition - newPiecePosition);
				if (distance == 2) {
					isCastle = true;
				}
			}
			return isCastle;
		}
		private Tuple<int, int> getRookPositionsForCastle(ChessType.Color color, int piecePosition, int newPiecePosition) {
			//manage the castle
			var rookRank = color == ChessType.Color.White ? 1 : 8; //intentionally not zero based
			var rookFile = CoordinateService.IntToFile(piecePosition - newPiecePosition > 0 ? 0 : 7);
			var rookPos = CoordinateService.CoordinateToPosition(string.Concat(rookFile, rookRank.ToString()));

			var newRookFile = CoordinateService.IntToFile(piecePosition - newPiecePosition > 0 ? 3 : 5);
			var newRookPos = CoordinateService.CoordinateToPosition(string.Concat(newRookFile, rookRank.ToString()));

			return Tuple.Create<int, int>(rookPos, newRookPos);
		}
		/// <summary>
		/// In chess, in order for a position to be considered the same, each player must have the same set of legal moves each time, 
		/// including the possible rights to castle and capture en passant. Positions are considered the same if the same type of piece 
		/// is on a given square. So, for instance, if a player has two knights and the knights are on the same squares, it does not 
		/// matter if the positions of the two knights have been exchanged. The game is not automatically drawn if a position occurs 
		/// for the third time – one of the players, on their move turn, must claim the draw with the arbiter.
		/// </summary>
		/// <returns></returns>
		private bool hasThreefoldRepition(ChessType.Color color, Board board, List<BoardHistory> boardHistory, string newFEN) {
			if(board.HasThreefoldRepition) { return true; }

			var historySize = boardHistory.Count();
			if(historySize > 5) {
				var newBoardHistory = new List<BoardHistory>();
				boardHistory.ForEach(b => { newBoardHistory.Add(b); });
				var newBoard = getNewBoard(newFEN, board.PGN, board.HasThreefoldRepition, string.Empty);
				var newHistory = GetBoardHistory(newBoard);
				newBoardHistory.Add(newHistory);

				var threeIdentical = newBoardHistory
											.GroupBy(a => new { a.Position, a.CastlingAvailability, a.EnPassantTargetSquare })
											.Where(a => a.Count() >= 3)
											.Select(a => new { a.Key.Position, a.Key.CastlingAvailability, a.Key.EnPassantTargetSquare });
				if(threeIdentical != null && threeIdentical.Any()) { return true; }
			}

			return false;
		}
		private bool isCheckmate(ChessType.Color activeChessTypeColor, Dictionary<int, char> matrix, int enemyKingSquare, Dictionary<int, List<int>> whiteAttacks, Dictionary<int, List<int>> blackAttacks) {
			bool kingIsBeingAttacked = whiteAttacks.ContainsKey(enemyKingSquare) || blackAttacks.ContainsKey(enemyKingSquare);
			if(kingIsBeingAttacked) {
				//make sure that he cannot move
				bool kingHasEscape = false;

				var friendlyAttacks = (activeChessTypeColor == ChessType.Color.White ? whiteAttacks : blackAttacks);
				var opponentAttacks = (activeChessTypeColor == ChessType.Color.White ? blackAttacks : whiteAttacks);

				var kingAttacks = friendlyAttacks.Where(a => a.Key == enemyKingSquare).FirstOrDefault().Value;
				var opponentFlatAttacks = opponentAttacks.SelectMany(a => a.Value);

				var remainingKingAttacks = kingAttacks.Except(opponentFlatAttacks);
				if(remainingKingAttacks.Any()) {
					kingHasEscape = true; 
				}
				if(!kingHasEscape) { //make sure that interposition is not possible
					var attackers = opponentAttacks.Where(a => a.Value.Contains(enemyKingSquare));
					if(attackers != null && attackers.Count() == 1) { //if there is more than one attacker there cannot be an interposition that saves the king...i think
						var attacker = attackers.FirstOrDefault();
						var attackerPiece = matrix.Where(a => a.Key == attacker.Key).FirstOrDefault().Value;
						var attackerPieceType = CoordinateService.GetPieceTypeFromChar(attackerPiece);
						var attackerPieceColor = CoordinateService.GetColorFromChar(attackerPiece);

						var theAttack = getAttack(attackerPieceColor, matrix, attacker.Key, enemyKingSquare, attackerPieceType);

						var friendlyFlatAttacks = friendlyAttacks.SelectMany(a => a.Value);
						var interposers = friendlyFlatAttacks.Intersect(theAttack);
						if(interposers.Any()) { return false; } 							
					}
					return true; //there were no friendlies to save the king, checkmate is true
				}
			}
			return false;
		}
		private static List<int> getAttack(ChessType.Color attackerPieceColor, Dictionary<int, char> matrix, int attackerPosition, int enemyKingSquare, ChessType.PieceType attackerPieceType) {
			var theAttack = new List<int>();
			switch(attackerPieceType) {
				case ChessType.PieceType.Pawn | ChessType.PieceType.Knight | ChessType.PieceType.King: //you can't interpose a pawn or a knight attack, also a king cannot attack a king
					break;
				case ChessType.PieceType.Bishop:
					foreach(var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if(potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;
				case ChessType.PieceType.Rook:
					foreach(var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if(potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;
				case ChessType.PieceType.Queen:
					foreach(var direction in CoordinateService.DiagonalLines) {
						var potentialAttack = CoordinateService.GetDiagonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if(potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					foreach(var direction in CoordinateService.OrthogonalLines) {
						var potentialAttack = CoordinateService.GetOrthogonalLine(matrix, attackerPosition, direction, attackerPieceColor, true);
						if(potentialAttack.Contains(enemyKingSquare)) {
							theAttack.AddRange(potentialAttack);
							break;
						}
					}
					break;
			}
			return theAttack;
		}
	}
}