var GameImage = (function () {
	var root = (function () {
		var baseUrl = window.location.protocol + "//" + window.location.host;
		if (baseUrl.indexOf('localhost') >= 0) {
			if (baseUrl.indexOf(':' >= 0)) {
				return baseUrl;
			} else {
				var pathArray = window.location.pathname.split('/');
				var host = pathArray[1];
				return baseUrl + "/" + host;
			}
		}
		return baseUrl;
	})();
	return function (url, width, height) {
		var self = this;
		self.image = new Image();
		self.rotation = 0;

		if (!width) {
			width = self.image.width;
		}
		if (!height) {
			height = self.image.height;
		}

		self.shape = new NinjaJS.Shape(function () {
			if (self.image.width === 0) { return; }

			var canvas = this.getCanvas();
			canvas.width = width;
			canvas.height = height;
			var ctx = this.getContext();
			ctx.rect(0, 0, width, height);
			if (self.rotation !== 0) {
				ctx.rotate(self.rotation * Math.PI / 180);
			}
			ctx.drawImage(self.image, 0, 0, width, height);

		});
		self.rotate = function (rot) {
			self.rotation += rot;
			if (self.rotation > 360) {
				self.rotation = Math.abs(self.rotation - 360);
			}
			self.shape.redraw();
		};
		self.load = function (onload) {
			self.image.onload = (function () {
				self.shape.redraw();
				onload();
			});
			self.image.src = root + url;
		}
	}
})();

var game = {
	Id: 0,
	CanvasId: 0,
	ScoreBoardId: 0,
	PlayerColor: '-',
	ChessBoard: null,
	ThisMove: {
		StartSquare: null,
		EndSquare: null
	},
	Stage: null,
	Canvas: null,
	ScoreBoard: null,
	CanvasMinX: 0,
	CanvasMinY: 0,
	CanvasWidth: 400,
	SquareSize: 50,
	ChessHub: null,

	/// <summary>Initializes the game.</summary>
	/// <param name="canvasId">DOM id of the div tag that will contain the canvas.</param>
	/// <param name="scoreBoardId">DOM id of the div tag that will contain the score board.</param>
	/// <param name="gameId">Integer that represents the identifier for the game after all participants have accepted the challenge.</param>
	/// <param name="playerColor">w or b to designate turn.</param>
	/// <returns>void</returns>
	Init: function (canvasId, scoreBoardId, gameId, playerColor, userId) {
		$('#statusBar').hide();
		this.Id = gameId;
		this.CanvasId = canvasId;
		this.ScoreBoardId = scoreBoardId;
		this.PlayerColor = playerColor;
		this.Stage = new NinjaJS.Stage(document.getElementById(canvasId), this.CanvasWidth, this.CanvasWidth);
		this.Canvas = this.Stage.canvas;
		this.ScoreBoard = this.getScoreBoard();
		var isInit = gameId == 0 ? true : false;
		this.setupSignalR(userId, isInit);
	},
	setupSignalR: function (userId, isInit) {
		var theGame = this;
		// Declare a proxy to reference the hub. 
		ChessHub = $.connection.chessHub;
		ChessHub.state.userId = userId;
		ChessHub.client.displayGameStatus = function (data, isInit) {
			theGame.setGameStatus(data, isInit);
		}
		// Start the connection.
		$.connection.hub.start().done(function () {
			ChessHub.server.get(theGame.Id);
		});
	},
	setGameStatus: function (chessGame, isInit) { //this method will call the draw methods, because they are dependent on the game status
		if (this.Id == 0) {
			if (chessGame.GameId > 0) {
				this.Id = chessGame.GameId;
			} else if(!isInit){ alert("Error updating ID!"); }
		}
		this.ChessBoard = chessGame;
		this.refreshBoard();
		if (!this.ChessBoard.MoveSuccess) {
			//todo: make this pretty
			alert(this.ChessBoard.MoveFailureMessage);
		}
	},
	refreshBoard: function () {
		this.Stage.clear();
		this.resetThisMove();
		this.drawBoard();
		this.drawPieces();
		this.setScoreBoard(this.ChessBoard.ActiveColor, this.ChessBoard.FEN, this.ChessBoard.PGN);
		this.Stage.draw();
	},
	validateMove: function (startSquare, endSquare) {
		var url = "/api/chess/?id=" + this.Id + "&activeColor=" + this.ChessBoard.ActiveColor + "&startSquare=" + startSquare + "&endSquare=" + endSquare;
		if (!startSquare || !endSquare) {
			this.refreshBoard();
			return;
		}

		var isPawnPromotion = this.isThisAPawnPromotion(this.ChessBoard.ActiveColor, this.ChessBoard.Matrix, startSquare, endSquare);
		if (!isPawnPromotion) {
			ChessHub.server.move(this.Id, this.ChessBoard.ActiveColor, startSquare, endSquare);
			//this.sendValidation(url);
		} else {
			this.showPawnPromotionDialog(this.Id, this.ChessBoard.ActiveColor, startSquare, endSquare);
		}
	},
	sendValidation: function (url) {
		var theGame = this;
		$.ajax({
			url: url,
			context: document.body,
			success: function (data) {
				theGame.setGameStatus(data, false);
			},
			error: function (data) {
				theGame.refreshBoard();
				theGame.invalidMoveAlert();
			}
		});
	},
	isThisAPawnPromotion: function (activeColor, matrix, startSquare, endSquare) {
		//do we need a pawn promotion?
		var startPos = this.squareToPosition(startSquare);
		var endPos = this.squareToPosition(endSquare);
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
	},
	resetThisMove: function () {
		this.ThisMove.StartSquare = null;
		this.ThisMove.EndSquare = null;
	},
	getScoreBoard: function () {
		return document.getElementById(this.ScoreBoardId);
	},
	setScoreBoard: function (activeColor, fen, pgn) {
		var yourMove = this.PlayerColor == this.ChessBoard.ActiveColor ? true : false;
		var moveIndicator = '<span id="moveIndicator">' + (activeColor == 'w' ? 'White\'s move' : 'Black\'s move') + (yourMove ? ' - You\'re up!' : '') + '</span>';
		var fenDisplay = '<span>' + fen + '</span>';
		var pgnData = "";
		if (this.ChessBoard.PGN) {
			pgnData = '<span>' + pgn + '</span>';
		}
		$('#MoveIndicator').html(moveIndicator);
		$('#FEN').html(fenDisplay);
		$('#PGN').html(pgnData + '<br clear="both" />');
		$('#statusBar').show();
	},
	isNewGame: function () {
		if (this.ChessBoard.FullmoveNumber == 1 && this.ChessBoard.ActiveColor == 'w') {
			return true;
		}
		return false;
	},
	findSquare: function (x, y) {
		var rank = 8 - (y / this.SquareSize);
		var file = (x / this.SquareSize) + 1;
		var square = this.convertFileToLetter(file) + rank;
		return square;
	},
	findPosition: function (position) {
		var rank = parseInt(position / 8) + 1;
		var file = (position % 8) + 1;
		var y = 400 - (rank * this.SquareSize);
		var x = (file - 1) * this.SquareSize;
		var retval = new Array(x, y);
		return retval;
	},
	getImageName: function (char) {
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
	},
	convertFileToLetter: function (file) {
		return String.fromCharCode(64 + file).toLowerCase();
	},
	convertLetterToFile: function (letter) {
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
	},
	squareToPosition: function (square) {
		var file = this.convertLetterToFile(square[0]);
		var rank = square[1];
		return ((rank - 1) * 8) + (file - 1);
	},
	invalidMoveAlert: function (message) {
		if (message) {
			alert(message);
		} else {
			alert("Invalid move, try again.");
		}
	},
	drawBoard: function () {
		var posX = 0;
		var posY = 0;
		var squareSize = this.SquareSize;
		var alt = false;
		var fillStyle = "";
		for (var i = 0; i < 8; i++) {
			for (var j = 0; j < 8; j++) {
				fillStyle = alt ? "rgb(71,71,71)" : "white";
				posX = this.SquareSize * j;
				posY = this.SquareSize * i;
				var shape = this.getNinjaSquareShape(squareSize, squareSize, fillStyle, posX, posY);
				this.Stage.add(shape);
				alt = alt ? false : true;
			}
			alt = alt ? false : true;
		}
	},
	getNinjaSquareShape: function (height, width, fillStyle, x, y) {
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
	},
	drawPieces: function () { //then display the board
		var matrix = this.ChessBoard.Matrix;
		for (x in matrix) {
			var position = x;
			var piece = matrix[x];
			this.drawImage(this.getImageName(piece), this.findPosition(position));
		}
	},
	drawImage: function (colorPiece, coordinate) {
		var squareSize = this.SquareSize;
		var self = this;

		var size = Math.floor(squareSize * 0.95);
		var x = coordinate[0] + (squareSize - size);
		var y = coordinate[1] + (squareSize - size);

		var imgUrl = self.getPieceSource(colorPiece);
		var gameImage = new GameImage(imgUrl, size, size);
		gameImage.shape.setPosition(x, y);
		gameImage.shape.draggable = true;
		gameImage.shape.bind('dragstart', function (e) { self.onDragStart(e, gameImage); });
		gameImage.shape.bind('dragend', function (e) { self.onDragEnd(e); });
		this.Stage.add(gameImage.shape);
		gameImage.load(function () {
			self.Stage.draw();
		});
	},
	onDragStart: function (e, gameImage) {
		var square = this.getSquareFromMousePosition(this, e.userPosition.x, e.userPosition.y);

		//these were used before we knew what color the current player was
		//var position = this.squareToPosition(square);
		//var piece = this.ChessBoard.Matrix[position];
		//var pieceColor = piece == piece.toUpperCase() ? 'w' : 'b';

		if (this.PlayerColor == this.ChessBoard.ActiveColor) {
			this.ThisMove.StartSquare = square;
		} else {
			alert('Please wait for your opponent to move before you move again.');
			gameImage.shape.trigger('dragend', e);
			this.ThisMove.StartSquare = null;
			return;
		}
	},
	onDragEnd: function (e) {
		var square = this.getSquareFromMousePosition(this, e.userPosition.x, e.userPosition.y);
		this.ThisMove.EndSquare = square;
		this.validateMove(this.ThisMove.StartSquare, this.ThisMove.EndSquare);
	},
	getPieceSource: function (colorPieceString) {
		var url = '/Images/Pieces/' + colorPieceString + '.png';
		return url;
	},
	isUpperCase: function (myString) {
		return (myString == myString.toUpperCase());
	},
	getSquareFromMousePosition: function (self, x, y) {
		var _x = self.roundDown(Math.floor(x));
		var _y = self.roundDown(Math.floor(y));
		var square = self.findSquare(_x, _y);
		return square;
	},
	roundDown: function (val) {
		if (val) {
			var r = parseInt(val / this.SquareSize);
			r = r * this.SquareSize
			return r;
		}
		return val;
	},
	showPawnPromotionDialog: function (id, activeColor, startSquare, endSquare) {
		var baseAction = "game.promoteTo(\'" + id + "', '" + activeColor + "', '" + startSquare + "', '" + endSquare + "\', \'[[piece]]\')";
		var queenAction = baseAction.replace("[[piece]]", "q");
		var knightAction = baseAction.replace("[[piece]]", "n"); 
		var pawnPromoteDialog = '<div>Choose the piece you\'d like: <button onclick="' + queenAction + '">Queen</button> <button onclick="' + knightAction + '">Knight</button></div>';
		$.fancybox(pawnPromoteDialog);
	},
	promoteTo: function (url, piece) {
		$.fancybox.close();
		//url += '&promoteToPiece=' + piece;
		ChessHub.server.promote(id, activeColor, startSquare, endSquare, piece)
		//this.sendValidation(url);		
	}
}