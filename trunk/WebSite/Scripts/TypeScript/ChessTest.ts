/// <referencxe path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/fancybox/fancybox.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/ninjajs/ninjajs.d.ts" />
/// <reference path="ninjajs.ts" />

module TalariusChess {
	export class Game {
		private ChessGame: ChessGame;
		private ChessBoardHelper: ChessBoardHelper;
		private CanvasWidth: number = 400;
		public static HubCom: HubCom;

		constructor(canvasId: string, scoreBoardId: string, gameId: number, playerColor: string, userId: number) {
			this.ChessGame = new ChessGame(gameId, playerColor, scoreBoardId);
			var canvas = document.getElementById(canvasId);
			this.ChessGame.Stage = new NinjaJS.Stage(canvas, this.CanvasWidth, this.CanvasWidth, null, null);
			this.ChessBoardHelper = new ChessBoardHelper(this.ChessGame);
			Game.HubCom = new HubCom(gameId, this.ChessBoardHelper, userId);
		}
	}
	export class ChessGame {
		public Id: number = 0;
		public PlayerColor: string = '-';
		public ChessBoard = null;
		public Stage: any = null;
		public ScoreBoard = null;
		public CurrentMove: Move;

		constructor(gameId: number, playerColor: string, scoreBoardId: string) {
    		this.Id = gameId;
    		this.PlayerColor = playerColor;
    		this.ScoreBoard = document.getElementById(scoreBoardId);
    	}
	}
	export class HubCom {
		public ChessBoardHelper: ChessBoardHelper;
		public ChessHub;

		constructor(gameId: number, chessBoardHelper: ChessBoardHelper, userId: number) {
			this.ChessBoardHelper = chessBoardHelper;

			// Declare a proxy to reference the hub. 
			this.ChessHub = $.connection.hub;
			this.ChessHub.state.userId = userId;
			this.ChessHub.client.displayGameStatus = function (data) {
				this.ChessBoardHelper.setGameStatus(data);
			}
			// Start the connection.
			$.connection.hub.start().done(function () {
				this.ChessHub.server.get(gameId);
			});
		}
		sendMoveToServer() {
			this.ChessHub.server.move(this.ChessBoardHelper.Game.Id, this.ChessBoardHelper.Game.ChessBoard.ActiveColor, this.ChessBoardHelper.Game.CurrentMove.StartSquare, this.ChessBoardHelper.Game.CurrentMove.EndSquare);
		}
		sendPromotionMoveToServer(id: string, activeColor: string, startSquare: string, endSquare: string, piece: string) {
			$.fancybox.close();
			this.ChessHub.server.promote(id, activeColor, startSquare, endSquare, piece)
		}
	}
	export class ChessBoardHelper {
		public Game: ChessGame;
		public SquareSize: number = 50;
		public PieceSize: number = 0;
		public BlackOnBottom: bool = false; //should control the orientation of the board
		public Drawing: Drawing;
		public Conversion;

		constructor(game: ChessGame) {
			$('#statusBar').hide();
			this.Game = game;
			this.PieceSize = Math.floor(this.SquareSize * 0.95);
			this.Drawing = new Drawing(this.Game, this.SquareSize, this.PieceSize);
			this.Conversion = new Conversion(this.SquareSize);
		}

		//events
		onDragStart(e, gameImage: GameImage) {
			var x:number = e.userPosition.x;
			var y: number = e.userPosition.y;
			this.validateMoveBegin(e, x, y, gameImage);
		}
		onDragEnd(e) {
			var x: number = e.userPosition.x;
			var y: number = e.userPosition.y;
			this.validateMoveEnd(x, y);
		}

		//this only gets called by the signalR hub
		setGameStatus(chessGame: ChessGame) {
			//this method will call the draw methods, because they are dependent on the game status
			this.refreshBoard(chessGame);
			if (!chessGame.ChessBoard.MoveSuccess) { //todo: make this pretty
				Messages.custom(chessGame.ChessBoard.MoveFailureMessage);
			}
		}
		refreshBoard(chessGame: ChessGame) {
			chessGame.CurrentMove = new Move(null, null);
			this.Drawing.drawBoard(chessGame, this.onDragStart, this.onDragEnd);
		}

		//move validation and server communication
		validateMoveBegin(e, x:number, y:number, gameImage:GameImage) {
			var isMoveValid = this.Game.PlayerColor == this.Game.ChessBoard.ActiveColor;
			if (isMoveValid) {
				this.Game.CurrentMove.StartSquare = this.Conversion.getSquareFromMousePosition(x, y);
			} else {
				gameImage.Shape.trigger('dragend', e);
				this.Game.CurrentMove.StartSquare = null;
				Messages.itIsNotYourTurn();
			}
		}
		validateMoveEnd(x: number, y: number) {
			this.Game.CurrentMove.EndSquare = this.Conversion.getSquareFromMousePosition(x, y);
			if (!this.Game.CurrentMove.isValid()) {
				this.refreshBoard(this.Game.ChessBoard);
				Messages.invalidMove();
				return;
			}
			var isPawnPromotion: bool = this.isThisAPawnPromotion(this.Game.ChessBoard.ActiveColor, this.Game.ChessBoard.Matrix, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);

			var isPawnPromotion: bool = this.isThisAPawnPromotion(this.Game.ChessBoard.ActiveColor, this.Game.ChessBoard.Matrix, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);
			if (!isPawnPromotion) {
				Game.HubCom.sendMoveToServer();
			} else {
				this.showPawnPromotionDialog(this.Game.Id, this.Game.ChessBoard.ActiveColor, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);
			}
		}
		//pawn promotion
		isThisAPawnPromotion(activeColor, matrix, startSquare, endSquare): bool {
			//do we need a pawn promotion?
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
		}
		showPawnPromotionDialog(id, activeColor, startSquare, endSquare) {
			var baseAction = "Game.HubCom.sendPromotionMoveToServer(\'" + id + "', '" + activeColor + "', '" + startSquare + "', '" + endSquare + "\', \'[[piece]]\')";
			var queenAction = baseAction.replace("[[piece]]", "q");
			var knightAction = baseAction.replace("[[piece]]", "n");
			var pawnPromoteDialog = '<div>Choose the piece you\'d like: <button onclick="' + queenAction + '">Queen</button> <button onclick="' + knightAction + '">Knight</button></div>';
			$.fancybox(pawnPromoteDialog);
		}
	}
	export class Conversion {
		public SquareSize: number;

		constructor(squareSize: number) {
			this.SquareSize = squareSize;
		}

		getSquareFromMousePosition(x: number, y: number) {
			var _x = this.roundDown(Math.floor(x));
			var _y = this.roundDown(Math.floor(y));
			var square = this.findSquare(_x, _y, this.SquareSize);
			return square;
		}
		findSquare(x: number, y: number, squareSize: number): string {
			var rank = 8 - (y / squareSize);
			var file = (x / squareSize) + 1;
			var square = this.convertFileToLetter(file) + rank.toString();
			return square;
		}
		convertFileToLetter(file: number): string {
			return String.fromCharCode(64 + file).toLowerCase();
		}
		roundDown(val): number {
			if (val) {
				var r = +(val / this.SquareSize);
				r = r * this.SquareSize
				return r;
			}
			return val;
		}
		squareToPosition(square): number {
			var file = this.convertLetterToFile(square[0]);
			var rank = square[1];
			return ((rank - 1) * 8) + (file - 1);
		}
		convertLetterToFile(letter): number {
			var num = 0;
			switch (letter) {
				case 'a':
					num = 1;
					break;
				case 'b':
					num = 2;
					break;
				case 'c':
					num = 3;
					break;
				case 'd':
					num = 4;
					break;
				case 'e':
					num = 5;
					break;
				case 'f':
					num = 6;
					break;
				case 'g':
					num = 7;
					break;
				case 'h':
					num = 8;
					break;
			}
			return num;
		}
	}
	export class Messages {
		public static custom(message: string) {
			if (message) {
				alert(message);
			}
		}
		public static invalidMove() {
			alert("Invalid move, try again.");
		}
		public static itIsNotYourTurn() {
			alert('Please wait for your opponent to move before you move again.');
		}
	}
	export class Drawing {
		private SquareSize: number = 0;
		private PieceSize: number = 0;
		private CanvasWidth: number = 0;

		constructor(squareSize, pieceSize, canvasWidth) {
			this.SquareSize = squareSize;
			this.PieceSize = pieceSize;
			this.CanvasWidth = canvasWidth;
		}

		drawImage(position: number, piece: string, onDragStart: Function, onDragEnd: Function): GameImage {
			var imageName: string = this.getImageName(piece);
			var coordinate: number[] = this.findPosition(position);
			var diff: number = (this.SquareSize - this.PieceSize);
			var xCoord: number = coordinate[0] + diff;
			var yCoord: number = coordinate[1] + diff;
			var imgUrl = this.getPieceSource(imageName);
			var gameImage = new GameImage(imgUrl, this.PieceSize, this.PieceSize);
			gameImage.Shape.setPosition(xCoord, yCoord);
			gameImage.Shape.draggable = true;
			if (onDragStart) {
				gameImage.Shape.bind('dragstart', function (e) { onDragStart(e, gameImage); });
			}
			if (onDragEnd) {
				gameImage.Shape.bind('dragend', function (e) { onDragEnd(e); });
			}
			return gameImage;
		}
		drawBoard(chessGame: ChessGame, onDragStart: Function, onDragEnd: Function) {
			chessGame.Stage.clear();
			var posX = 0;
			var posY = 0;
			var alt = false;
			var fillStyle = "";
			for (var i = 0; i < 8; i++) {
				for (var j = 0; j < 8; j++) {
					fillStyle = alt ? "rgb(71,71,71)" : "white";
					posX = this.SquareSize * j;
					posY = this.SquareSize * i;
					var shape = this.getNinjaSquareShape(this.SquareSize, this.SquareSize, fillStyle, posX, posY);
					chessGame.Stage.add(shape);
					alt = alt ? false : true;
				}
				alt = alt ? false : true;
			}
			this.drawPieces(chessGame, onDragStart, onDragEnd);
			this.setScoreBoard(chessGame.PlayerColor, chessGame.ChessBoard.ActiveColor, chessGame.ChessBoard.FEN, chessGame.ChessBoard.PGN);
			chessGame.Stage.draw();
		}
		setScoreBoard(playerColor: string, activeColor: string, fen: string, pgn: string) {
			var yourMove = playerColor == activeColor ? true : false;
			var moveIndicator = '<span id="moveIndicator">' + (activeColor == 'w' ? 'White\'s move' : 'Black\'s move') + (yourMove ? ' - You\'re up!' : '') + '</span>';
			var fenDisplay = '<span>' + fen + '</span>';
			var pgnData = "";
			if (pgn) {
				pgnData = '<span>' + pgn + '</span>';
			}
			$('#MoveIndicator').html(moveIndicator);
			$('#FEN').html(fenDisplay);
			$('#PGN').html(pgnData + '<br clear="both" />');
			$('#statusBar').show();
		}
		drawPieces(chessGame: ChessGame, onDragStart: Function, onDragEnd: Function) { //then display the board
			var matrix = chessGame.ChessBoard.Matrix;
			for (var x in matrix) {
				var position: number = +x;
				var piece: string = matrix[x];
				var gameImage = this.drawImage(position, piece, onDragStart, onDragEnd);
				chessGame.Stage.add(gameImage.Shape);
			}
		}

		getNinjaSquareShape(height: number, width: number, fillStyle: string, x: number, y: number) {
			var shape = new NinjaJS.Shape(function () {
				var canvas = this.getCanvas();
				canvas.width = height;
				canvas.height = width;
				var ctx = this.getContext();
				ctx.fillStyle = fillStyle;
				ctx.fillRect(0, 0, height, width);
				ctx.strokeRect(0, 0, height, width);
			});
			shape.setPosition(x, y);
			return shape;
		}
		getPieceSource(colorPiece: string) {
			var url = '/Images/Pieces/' + colorPiece + '.png';
			return url;
		}
		getImageName(char: string): string {
			var color = this.isUpperCase(char) ? "White" : "Black";
			var piece = char.toUpperCase();
			var pieceName = "Pawn";
			switch (piece) {
				case "R":
					pieceName = "Rook"
					break;
				case "N":
					pieceName = "Knight"
					break;
				case "B":
					pieceName = "Bishop"
					break;
				case "Q":
					pieceName = "Queen"
					break;
				case "K":
					pieceName = "King"
					break;
			}
			return color + pieceName;
		}
		findPosition(position: number): any[] {
			var rank = +(position / 8) + 1;
			var file = (position % 8) + 1;
			var y = this.CanvasWidth - (rank * this.SquareSize);
			var x = (file - 1) * this.SquareSize;
			var retval = new Array(x, y);
			return retval;
		}
		isUpperCase(val: string): bool {
			return (val == val.toUpperCase());
		}
	}
	export class GameImage {
		public Image;
		public Width;
		public Height;
		public Url;
		public Rotation: number = 0;
		public Shape: Shape;

		constructor(url: string, width: number, height: number) {
			this.Image = new Image();
			if (!width) {
				this.Width = this.Image.width;
			}
			if (!height) {
				this.Height = this.Image.height;
			}
			this.Url = url;
			this.Shape = new NinjaJS.Shape(function () {
				if (this.Image.width === 0) { return; }

				var canvas = this.getCanvas();
				canvas.width = width;
				canvas.height = height;
				var ctx = this.getContext();
				ctx.rect(0, 0, width, height);
				if (this.Rotation !== 0) {
					ctx.rotate(this.Rotation * Math.PI / 180);
				}
				ctx.drawImage(this.Image, 0, 0, width, height);
			});
		}

		rotate(rot:number) {
			this.Rotation += rot;
			if (this.Rotation > 360) {
				this.Rotation = Math.abs(this.Rotation - 360);
			}
			this.Shape.redraw();
		}
		load() {
			this.Image.onload = (function () {
				this.shape.redraw();
				this.image.onload();
			});
			this.Image.src = this.getRoot() + this.Url;
		}		
		getRoot() {
			var baseUrl: string = window.location.protocol + "//" + window.location.host;
			if (baseUrl.indexOf('localhost') >= 0) {
				if (baseUrl.indexOf(':') >= 0) {
					return baseUrl;
				} else {
					var pathArray = window.location.pathname.split('/');
					var host = pathArray[1];
					return baseUrl + "/" + host;
				}
			}
			return baseUrl;
		}
	}
	export class Move {
		public StartSquare: string = '';
		public EndSquare: string = '';

		constructor(startSquare: string, endSquare: string) {
			this.StartSquare = startSquare;
			this.EndSquare = endSquare;
		}

		isValid() {
			return !this.StartSquare || !this.EndSquare;
		}
	}
}