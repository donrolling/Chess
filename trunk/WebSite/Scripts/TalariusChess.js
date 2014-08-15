var TalariusChess;
(function (TalariusChess) {
	var Game = (function () {
		function Game(canvasId, scoreBoardId, gameId, playerColor, userId, fen, matrix) {
			this.GameId = gameId;
			this.ScoreBoardId = scoreBoardId;
			this.UserId = userId;
			this.setupHub(userId, gameId);

			//setup page elements
			$('#statusBar').hide();
			this.ScoreBoard = document.getElementById(scoreBoardId);

			this.ChessGame = new ChessGame(gameId, playerColor, canvasId, fen, matrix, this);
		}

		Game.isThisAPawnPromotion = function (activeColor, matrix, startSquare, endSquare) {
			var startPos = this.Conversion.squareToPosition(startSquare);
			var endPos = this.Conversion.squareToPosition(endSquare);
			var onRightRank = false;
			if (activeColor == 'w' && endPos >= 56 && endPos <= 63) {
				onRightRank = true;
			} else if (activeColor == 'b' && endPos >= 0 && endPos <= 7) {
				onRightRank = true;
			}
			if (onRightRank) {
				var piece = matrix[startPos];
				if (piece.toUpperCase() == 'P') {
					return true;
				}
			}
			return false;
		};
		Game.showPawnPromotionDialog = function (id, activeColor, startSquare, endSquare) {
			var baseAction = "talariusChess.Game.sendPromotionMoveToServer(\'" + id + "', '" + activeColor + "', '" + startSquare + "', '" + endSquare + "\', \'[[piece]]\')";
			var queenAction = baseAction.replace("[[piece]]", "q");
			var knightAction = baseAction.replace("[[piece]]", "n");
			var pawnPromoteDialog = '<div>Choose the piece you\'d like: <button onclick="' + queenAction + '">Queen</button> <button onclick="' + knightAction + '">Knight</button></div>';
		};

		Game.prototype.setupHub = function (userId){
			this.Hub = $.connection.hub;
			this.Hub.state.userId = userId;

			var self = this;
			this.Hub.proxies.chesshub.client.displayGameStatus = function (data, isInit) {
				self.ChessGame = new ChessGame(self.GameId, self.ChessGame.PlayerColor, self.ChessGame.CanvasId, data.FEN, data.Matrix, self);
			};
		};
		Game.prototype.sendMoveToServer = function () {
			this.Hub.server.move(this.ChessGame.Id, this.ChessGame.ActiveColor, this.ChessGame.Move.StartSquare, this.ChessGame.Move.EndSquare);
		};
		Game.prototype.sendPromotionMoveToServer = function (id, activeColor, startSquare, endSquare, piece) {
			this.Hub.server.promote(id, activeColor, startSquare, endSquare, piece);
		};

		return Game;
	})();
	TalariusChess.Game = Game;

	var ChessGame = (function () {
		function ChessGame(gameId, playerColor, canvasId, fen, matrix, game) {
			this.Id = gameId;
			this.CanvasId = canvasId;
			this.PlayerColor = playerColor;
			this.Move = new Move(null, null);
			this.FEN = fen;
			this.Matrix = matrix;
			console.log(fen);
			this.ActiveColor = fen.split(' ')[1];

			var cfg = {
				draggable: true,
				dropOffBoard: 'snapback', // this is the default
				position: fen.split(' ')[0],
				onDragStart: function (source, piece, position, orientation) {
					console.log(game.ChessGame.PlayerColor);
					console.log(game.ChessGame.ActiveColor);
					var isMoveValid = game.ChessGame.PlayerColor == game.ChessGame.ActiveColor;
					if (isMoveValid) {
						game.ChessGame.Move.StartSquare = source;
						return true;
					}

					game.ChessGame.Move.StartSquare = null;
					Messages.itIsNotYourTurn();
					return false;
				},
				onDrop: function (source, piece, position, orientation) {
					if (!game.ChessGame.Move.isValid()) {
						Messages.invalidMove();
						return 'snapback';
					}
					game.ChessGame.Move.EndSquare = source;
					//var isPawnPromotion = Game.isThisAPawnPromotion(this.ActiveColor, this.ChessBoard.Matrix, this.Move.StartSquare, this.Move.EndSquare);
					//todo: wtf here?
				},
			};
			this.ChessBoard = new ChessBoard(canvasId, cfg);
		}
		return ChessGame;
	})();
	TalariusChess.ChessGame = ChessGame;

	var Messages = (function () {
		function Messages() { }
		Messages.custom = function custom(message) {
			if (message) {
				alert(message);
			}
		};
		Messages.invalidMove = function invalidMove() {
			alert("Invalid move, try again.");
		};
		Messages.itIsNotYourTurn = function itIsNotYourTurn() {
			alert('Please wait for your opponent to move before you move again.');
		};
		return Messages;
	})();
	TalariusChess.Messages = Messages;

	var Move = (function () {
		function Move(startSquare, endSquare) {
			this.StartSquare = startSquare;
			this.EndSquare = endSquare;
		}
		Move.prototype.isValid = function () {
			return !this.StartSquare || !this.EndSquare;
		};
		return Move;
	})();
	TalariusChess.Move = Move;

})(TalariusChess || (TalariusChess = {}));