var TalariusChess;
(function (TalariusChess) {
    var Game = (function () {
        function Game(canvasId, scoreBoardId, gameId, playerColor, userId) {
            this.CanvasWidth = 400;
            this.ChessGame = new ChessGame(gameId, playerColor, scoreBoardId);
            this.ChessBoardHelper = new ChessBoardHelper(this.ChessGame);
            this.HubCom = new HubCom(gameId, this.ChessBoardHelper, userId);
        }
        return Game;
    })();
    TalariusChess.Game = Game;    
    var ChessGame = (function () {
        function ChessGame(gameId, playerColor, scoreBoardId) {
            this.Id = 0;
            this.PlayerColor = '-';
            this.ChessBoard = null;
            this.Stage = null;
            this.ScoreBoard = null;
            this.Id = gameId;
            this.PlayerColor = playerColor;
            this.ScoreBoard = document.getElementById(scoreBoardId);
        }
        return ChessGame;
    })();
    TalariusChess.ChessGame = ChessGame;    
    var HubCom = (function () {
        function HubCom(gameId, chessBoardHelper, userId) {
            this.ChessBoardHelper = chessBoardHelper;
            this.Hub = $.connection.hub;
            this.Hub.state.userId = userId;
            var self = this;
            this.Hub.proxies.chesshub.client.displayGameStatus = function (data, isInit) {
                self.ChessBoardHelper.setGameStatus(data);
            };
            $.connection.hub.start().done(function () {
                self.Hub.proxies.chesshub.server.get(gameId);
            });
        }
        HubCom.prototype.sendMoveToServer = function () {
            this.Hub.server.move(this.ChessBoardHelper.ChessGame.Id, this.ChessBoardHelper.ChessGame.ChessBoard.ActiveColor, this.ChessBoardHelper.ChessGame.CurrentMove.StartSquare, this.ChessBoardHelper.ChessGame.CurrentMove.EndSquare);
        };
        HubCom.prototype.sendPromotionMoveToServer = function (id, activeColor, startSquare, endSquare, piece) {
            this.Hub.server.promote(id, activeColor, startSquare, endSquare, piece);
        };
        return HubCom;
    })();
    TalariusChess.HubCom = HubCom;    
    var ChessBoardHelper = (function () {
        function ChessBoardHelper(chessGame) {
            this.SquareSize = 50;
            this.PieceSize = 0;
            this.BlackOnBottom = false;
            $('#statusBar').hide();
            this.ChessGame = chessGame;
            this.PieceSize = Math.floor(this.SquareSize * 0.95);
            this.Drawing = new Drawing(this.ChessGame, this.SquareSize, this.PieceSize);
            this.Conversion = new Conversion(this.SquareSize);
        }
        ChessBoardHelper.prototype.onDragStart = function (e, gameImage) {
            var x = e.userPosition.x;
            var y = e.userPosition.y;
            this.validateMoveBegin(e, x, y, gameImage);
        };
        ChessBoardHelper.prototype.onDragEnd = function (e) {
            var x = e.userPosition.x;
            var y = e.userPosition.y;
            this.validateMoveEnd(x, y);
        };
        ChessBoardHelper.prototype.setGameStatus = function (chessGame) {
            this.refreshBoard(chessGame);
            if(!chessGame.ChessBoard.MoveSuccess) {
                Messages.custom(chessGame.ChessBoard.MoveFailureMessage);
            }
        };
        ChessBoardHelper.prototype.refreshBoard = function (chessGame) {
            chessGame.CurrentMove = new Move(null, null);
            this.Drawing.drawBoard(chessGame, this.onDragStart, this.onDragEnd);
        };
        ChessBoardHelper.prototype.validateMoveBegin = function (e, x, y, gameImage) {
            var isMoveValid = this.ChessGame.PlayerColor == this.ChessGame.ChessBoard.ActiveColor;
            if(isMoveValid) {
                this.ChessGame.CurrentMove.StartSquare = this.Conversion.getSquareFromMousePosition(x, y);
            } else {
                this.ChessGame.CurrentMove.StartSquare = null;
                Messages.itIsNotYourTurn();
            }
        };
        ChessBoardHelper.prototype.validateMoveEnd = function (x, y) {
            this.ChessGame.CurrentMove.EndSquare = this.Conversion.getSquareFromMousePosition(x, y);
            if(!this.ChessGame.CurrentMove.isValid()) {
                this.refreshBoard(this.ChessGame.ChessBoard);
                Messages.invalidMove();
                return;
            }
            var isPawnPromotion = this.isThisAPawnPromotion(this.ChessGame.ChessBoard.ActiveColor, this.ChessGame.ChessBoard.Matrix, this.ChessGame.CurrentMove.StartSquare, this.ChessGame.CurrentMove.EndSquare);
            var isPawnPromotion = this.isThisAPawnPromotion(this.ChessGame.ChessBoard.ActiveColor, this.ChessGame.ChessBoard.Matrix, this.ChessGame.CurrentMove.StartSquare, this.ChessGame.CurrentMove.EndSquare);
        };
        ChessBoardHelper.prototype.isThisAPawnPromotion = function (activeColor, matrix, startSquare, endSquare) {
            var startPos = this.Conversion.squareToPosition(startSquare);
            var endPos = this.Conversion.squareToPosition(endSquare);
            var onRightRank = false;
            if(activeColor == 'w' && endPos >= 56 && endPos <= 63) {
                onRightRank = true;
            } else if(activeColor == 'b' && endPos >= 0 && endPos <= 7) {
                onRightRank = true;
            }
            if(onRightRank) {
                var piece = matrix[startPos];
                if(piece.toUpperCase() == 'P') {
                    return true;
                }
            }
            return false;
        };
        ChessBoardHelper.prototype.showPawnPromotionDialog = function (id, activeColor, startSquare, endSquare) {
            var baseAction = "Game.HubCom.sendPromotionMoveToServer(\'" + id + "', '" + activeColor + "', '" + startSquare + "', '" + endSquare + "\', \'[[piece]]\')";
            var queenAction = baseAction.replace("[[piece]]", "q");
            var knightAction = baseAction.replace("[[piece]]", "n");
            var pawnPromoteDialog = '<div>Choose the piece you\'d like: <button onclick="' + queenAction + '">Queen</button> <button onclick="' + knightAction + '">Knight</button></div>';
        };
        return ChessBoardHelper;
    })();
    TalariusChess.ChessBoardHelper = ChessBoardHelper;    
    var Conversion = (function () {
        function Conversion(squareSize) {
            this.SquareSize = squareSize;
        }
        Conversion.prototype.getSquareFromMousePosition = function (x, y) {
            var _x = this.roundDown(Math.floor(x));
            var _y = this.roundDown(Math.floor(y));
            var square = this.findSquare(_x, _y, this.SquareSize);
            return square;
        };
        Conversion.prototype.findSquare = function (x, y, squareSize) {
            var rank = 8 - (y / squareSize);
            var file = (x / squareSize) + 1;
            var square = this.convertFileToLetter(file) + rank.toString();
            return square;
        };
        Conversion.prototype.convertFileToLetter = function (file) {
            return String.fromCharCode(64 + file).toLowerCase();
        };
        Conversion.prototype.roundDown = function (val) {
            if(val) {
                var r = +(val / this.SquareSize);
                r = r * this.SquareSize;
                return r;
            }
            return val;
        };
        Conversion.prototype.squareToPosition = function (square) {
            var file = this.convertLetterToFile(square[0]);
            var rank = square[1];
            return ((rank - 1) * 8) + (file - 1);
        };
        Conversion.prototype.convertLetterToFile = function (letter) {
            var num = 0;
            switch(letter) {
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
        };
        return Conversion;
    })();
    TalariusChess.Conversion = Conversion;    
    var Messages = (function () {
        function Messages() { }
        Messages.custom = function custom(message) {
            if(message) {
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
    var Drawing = (function () {
        function Drawing(squareSize, pieceSize, canvasWidth) {
            this.SquareSize = 0;
            this.PieceSize = 0;
            this.CanvasWidth = 0;
            this.SquareSize = squareSize;
            this.PieceSize = pieceSize;
            this.CanvasWidth = canvasWidth;
        }
        Drawing.prototype.drawImage = function (position, piece, onDragStart, onDragEnd) {
            var imageName = this.getImageName(piece);
            var coordinate = this.findPosition(position);
            var diff = (this.SquareSize - this.PieceSize);
            var xCoord = coordinate[0] + diff;
            var yCoord = coordinate[1] + diff;
            var imgUrl = this.getPieceSource(imageName);
            var gameImage = new GameImage(imgUrl, this.PieceSize, this.PieceSize);
            return gameImage;
        };
        Drawing.prototype.drawBoard = function (chessGame, onDragStart, onDragEnd) {
            chessGame.Stage.clear();
            var posX = 0;
            var posY = 0;
            var alt = false;
            var fillStyle = "";
            for(var i = 0; i < 8; i++) {
                for(var j = 0; j < 8; j++) {
                    fillStyle = alt ? "rgb(71,71,71)" : "white";
                    posX = this.SquareSize * j;
                    posY = this.SquareSize * i;
                    alt = alt ? false : true;
                }
                alt = alt ? false : true;
            }
            this.drawPieces(chessGame, onDragStart, onDragEnd);
            this.setScoreBoard(chessGame.PlayerColor, chessGame.ChessBoard.ActiveColor, chessGame.ChessBoard.FEN, chessGame.ChessBoard.PGN);
            chessGame.Stage.draw();
        };
        Drawing.prototype.setScoreBoard = function (playerColor, activeColor, fen, pgn) {
            var yourMove = playerColor == activeColor ? true : false;
            var moveIndicator = '<span id="moveIndicator">' + (activeColor == 'w' ? 'White\'s move' : 'Black\'s move') + (yourMove ? ' - You\'re up!' : '') + '</span>';
            var fenDisplay = '<span>' + fen + '</span>';
            var pgnData = "";
            if(pgn) {
                pgnData = '<span>' + pgn + '</span>';
            }
            $('#MoveIndicator').html(moveIndicator);
            $('#FEN').html(fenDisplay);
            $('#PGN').html(pgnData + '<br clear="both" />');
            $('#statusBar').show();
        };
        Drawing.prototype.drawPieces = function (chessGame, onDragStart, onDragEnd) {
            var matrix = chessGame.ChessBoard.Matrix;
            for(var x in matrix) {
                var position = +x;
                var piece = matrix[x];
                var gameImage = this.drawImage(position, piece, onDragStart, onDragEnd);
            }
        };
        Drawing.prototype.getPieceSource = function (colorPiece) {
            var url = '/Images/Pieces/' + colorPiece + '.png';
            return url;
        };
        Drawing.prototype.getImageName = function (char) {
            var color = this.isUpperCase(char) ? "White" : "Black";
            var piece = char.toUpperCase();
            var pieceName = "Pawn";
            switch(piece) {
                case "R":
                    pieceName = "Rook";
                    break;
                case "N":
                    pieceName = "Knight";
                    break;
                case "B":
                    pieceName = "Bishop";
                    break;
                case "Q":
                    pieceName = "Queen";
                    break;
                case "K":
                    pieceName = "King";
                    break;
            }
            return color + pieceName;
        };
        Drawing.prototype.findPosition = function (position) {
            var rank = +(position / 8) + 1;
            var file = (position % 8) + 1;
            var y = this.CanvasWidth - (rank * this.SquareSize);
            var x = (file - 1) * this.SquareSize;
            var retval = new Array(x, y);
            return retval;
        };
        Drawing.prototype.isUpperCase = function (val) {
            return (val == val.toUpperCase());
        };
        return Drawing;
    })();
    TalariusChess.Drawing = Drawing;    
    var GameImage = (function () {
        function GameImage(url, width, height) {
            this.Rotation = 0;
            this.Image = new Image();
            if(!width) {
                this.Width = this.Image.width;
            }
            if(!height) {
                this.Height = this.Image.height;
            }
            this.Url = url;
        }
        GameImage.prototype.rotate = function (rot) {
            this.Rotation += rot;
            if(this.Rotation > 360) {
                this.Rotation = Math.abs(this.Rotation - 360);
            }
        };
        GameImage.prototype.load = function () {
            this.Image.onload = (function () {
                this.shape.redraw();
                this.image.onload();
            });
            this.Image.src = this.getRoot() + this.Url;
        };
        GameImage.prototype.getRoot = function () {
            var baseUrl = window.location.protocol + "//" + window.location.host;
            if(baseUrl.indexOf('localhost') >= 0) {
                if(baseUrl.indexOf(':') >= 0) {
                    return baseUrl;
                } else {
                    var pathArray = window.location.pathname.split('/');
                    var host = pathArray[1];
                    return baseUrl + "/" + host;
                }
            }
            return baseUrl;
        };
        return GameImage;
    })();
    TalariusChess.GameImage = GameImage;    
    var Move = (function () {
        function Move(startSquare, endSquare) {
            this.StartSquare = '';
            this.EndSquare = '';
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
