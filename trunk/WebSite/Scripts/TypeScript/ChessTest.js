var TalariusChess;
(function (TalariusChess) {
    var Game = (function () {
        function Game(canvasId, scoreBoardId, gameId, playerColor, userId) {
            this.CanvasWidth = 400;
            this.ChessGame = new ChessGame(gameId, playerColor, scoreBoardId);
            var canvas = document.getElementById(canvasId);
            this.ChessGame.Stage = new NinjaJS.Stage(canvas, this.CanvasWidth, this.CanvasWidth, null, null);
            this.ChessBoardHelper = new ChessBoardHelper(this.ChessGame);
            Game.HubCom = new HubCom(gameId, this.ChessBoardHelper, userId);
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
            this.ChessHub = $.connection.hub;
            this.ChessHub.state.userId = userId;
            this.ChessHub.client.displayGameStatus = function (data) {
                this.ChessBoardHelper.setGameStatus(data);
            };
            $.connection.hub.start().done(function () {
                this.ChessHub.server.get(gameId);
            });
        }
        HubCom.prototype.sendMoveToServer = function () {
            this.ChessHub.server.move(this.ChessBoardHelper.Game.Id, this.ChessBoardHelper.Game.ChessBoard.ActiveColor, this.ChessBoardHelper.Game.CurrentMove.StartSquare, this.ChessBoardHelper.Game.CurrentMove.EndSquare);
        };
        HubCom.prototype.sendPromotionMoveToServer = function (id, activeColor, startSquare, endSquare, piece) {
            $.fancybox.close();
            this.ChessHub.server.promote(id, activeColor, startSquare, endSquare, piece);
        };
        return HubCom;
    })();
    TalariusChess.HubCom = HubCom;    
    var ChessBoardHelper = (function () {
        function ChessBoardHelper(game) {
            this.SquareSize = 50;
            this.PieceSize = 0;
            this.BlackOnBottom = false;
            $('#statusBar').hide();
            this.Game = game;
            this.PieceSize = Math.floor(this.SquareSize * 0.95);
            this.Drawing = new Drawing(this.Game, this.SquareSize, this.PieceSize);
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
            var isMoveValid = this.Game.PlayerColor == this.Game.ChessBoard.ActiveColor;
            if(isMoveValid) {
                this.Game.CurrentMove.StartSquare = this.Conversion.getSquareFromMousePosition(x, y);
            } else {
                gameImage.Shape.trigger('dragend', e);
                this.Game.CurrentMove.StartSquare = null;
                Messages.itIsNotYourTurn();
            }
        };
        ChessBoardHelper.prototype.validateMoveEnd = function (x, y) {
            this.Game.CurrentMove.EndSquare = this.Conversion.getSquareFromMousePosition(x, y);
            if(!this.Game.CurrentMove.isValid()) {
                this.refreshBoard(this.Game.ChessBoard);
                Messages.invalidMove();
                return;
            }
            var isPawnPromotion = this.isThisAPawnPromotion(this.Game.ChessBoard.ActiveColor, this.Game.ChessBoard.Matrix, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);
            var isPawnPromotion = this.isThisAPawnPromotion(this.Game.ChessBoard.ActiveColor, this.Game.ChessBoard.Matrix, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);
            if(!isPawnPromotion) {
                Game.HubCom.sendMoveToServer();
            } else {
                this.showPawnPromotionDialog(this.Game.Id, this.Game.ChessBoard.ActiveColor, this.Game.CurrentMove.StartSquare, this.Game.CurrentMove.EndSquare);
            }
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
            $.fancybox(pawnPromoteDialog);
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
            gameImage.Shape.setPosition(xCoord, yCoord);
            gameImage.Shape.draggable = true;
            if(onDragStart) {
                gameImage.Shape.bind('dragstart', function (e) {
                    onDragStart(e, gameImage);
                });
            }
            if(onDragEnd) {
                gameImage.Shape.bind('dragend', function (e) {
                    onDragEnd(e);
                });
            }
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
                    var shape = this.getNinjaSquareShape(this.SquareSize, this.SquareSize, fillStyle, posX, posY);
                    chessGame.Stage.add(shape);
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
                chessGame.Stage.add(gameImage.Shape);
            }
        };
        Drawing.prototype.getNinjaSquareShape = function (height, width, fillStyle, x, y) {
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
            this.Shape = new NinjaJS.Shape(function () {
                if(this.Image.width === 0) {
                    return;
                }
                var canvas = this.getCanvas();
                canvas.width = width;
                canvas.height = height;
                var ctx = this.getContext();
                ctx.rect(0, 0, width, height);
                if(this.Rotation !== 0) {
                    ctx.rotate(this.Rotation * Math.PI / 180);
                }
                ctx.drawImage(this.Image, 0, 0, width, height);
            });
        }
        GameImage.prototype.rotate = function (rot) {
            this.Rotation += rot;
            if(this.Rotation > 360) {
                this.Rotation = Math.abs(this.Rotation - 360);
            }
            this.Shape.redraw();
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
