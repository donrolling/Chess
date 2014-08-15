using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chess.Model;
using Chess.ServiceLayer;

namespace Chess {
	public class Game {
		private BoardService _boardService;

		private bool _gameOver = false;
		public bool IsGameOver { get { return _gameOver; } }

		private GameResult _result = GameResult.InProgress;
		public GameResult Result  { get { return _result; } }

		public List<BoardHistory> BoardHistory = new List<BoardHistory>();

		public enum GameResult{
			White,
			Black,
			Draw,
			InProgress
		}

		public Board Board { get; set; }

		public Game() : this(FEN.StartingPosition) {}
		public Game(string fen) {
			_boardService = new BoardService();
			if(!string.IsNullOrEmpty(fen)){
				this.Board = _boardService.SetStartPosition(fen);
			} else { 
				this.Board = _boardService.SetStartPosition(FEN.StartingPosition);
			}
		}

		public void Move(ChessType.Color color, string pgnMove) {
			var squarePair = PGNService.PGNMoveToSquarePair(Board.Matrix, Board.AllAttacks, color, pgnMove);
			if(squarePair.Item1 == -1 || squarePair.Item2 == -1) {
				this.Board = _boardService.UpdateBoardWithError(this.Board, "Invalid move.");
				return;				
			}
			this.Move(color, squarePair.Item1, squarePair.Item2, pgnMove);
		}
		public void Move(ChessType.Color color, int piecePosition, int newPiecePosition, string pgnMove) {
			if (!_gameOver) {
				if(this.Board.ActiveChessTypeColor != color){
					this.Board = _boardService.UpdateBoardWithError(this.Board, string.Format("It is {0}'s move.", this.Board.ActiveChessTypeColor.ToString()));
					return; 
				}
				//perform the move
				var newBoard = _boardService.UpdateBoard(Board, color, piecePosition, newPiecePosition, pgnMove, BoardHistory);
				if (!newBoard.MoveSuccess) {
					this.Board = newBoard;
					return;
				}

				//save board history
				BoardHistory boardHistory = _boardService.GetBoardHistory(this.Board);
				this.BoardHistory.Add(boardHistory);
				this.Board = newBoard;

				//examine game status
				if (this.Board.HalfmoveClock == 100) { //100 fullmoves without a pawn move or capture is a draw
					EndGame(Game.GameResult.Draw);
					return;
				}

				//if is checkmate, then delcare the winner
				if(this.Board.IsCheckmate) {
					var winner = color == ChessType.Color.White ? GameResult.White : GameResult.Black;
					EndGame(winner);
					return;
				}

				//if is stalemate, then delcare the winner
				var isStalemate = false;
				if (isStalemate) {
					var winner = color == ChessType.Color.White ? GameResult.White : GameResult.Black;
					EndGame(winner);
					return;
				}

			}
		}

		public void Resign(ChessType.Color color) {
			if(this.Board.ActiveChessTypeColor != color) {
				this.Board = _boardService.UpdateBoardWithError(this.Board, string.Format("It is {0}'s move.", this.Board.ActiveChessTypeColor.ToString()));
				return;
			}
			switch(color) {
				case ChessType.Color.Black:
					this.EndGame(GameResult.White);
					break;
				case ChessType.Color.White:
					this.EndGame(GameResult.Black);
					break;
				case ChessType.Color.Invalid:
					this.Board = _boardService.UpdateBoardWithError(this.Board, string.Format("It is {0}'s move.", this.Board.ActiveChessTypeColor.ToString()));
					break;
			}
		}

		private void EndGame(GameResult gameResult) {
			_result = gameResult;
			_gameOver = true;
		}
	}
}
