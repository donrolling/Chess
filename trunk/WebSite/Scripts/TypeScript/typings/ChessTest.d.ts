/// <reference path="fancybox/fancybox.d.ts" />
/// <reference path="signalr/signalr.d.ts" />
/// <reference path="ninjajs/ninjajs.d.ts" />
module TalariusChess {
    class Game {
        public Id: number;
        public CanvasId: string;
        public ScoreBoardId: string;
        public PlayerColor: string;
        public ChessBoard;
        public Stage: any;
        public Canvas: any;
        public ScoreBoard;
        public CanvasMinX: number;
        public CanvasMinY: number;
        public CanvasWidth: number;
        public SquareSize: number;
        public ChessHub;
        public ThisMove: Move;
        private _chessBoardHelper;
        constructor(canvasId: string, scoreBoardId: string, gameId: number, playerColor: string, userId: number);
        public validateMove(startSquare, endSquare): void;
        public sendValidation(url: string): void;
        public resetThisMove(): void;
        public invalidMoveAlert(message): void;
        public drawBoard(): void;
    }
    class Move {
        public startSquare: number;
        public endSquare: number;
        public StartSquare: number;
        public EndSquare: number;
        constructor(startSquare: number, endSquare: number);
    }
    class ChessBoardHelper {
        public squareSize: number;
        public SquareSize: number;
        public Size: number;
        constructor(squareSize: number);
        public getNinjaSquareShape(height: number, width: number, fillStyle: string, x: number, y: number);
        public setGameStatus(chessGame: Game): void;
        public refreshBoard(chessGame: Game): void;
        public drawPieces(chessGame: Game): void;
        public getPieceSource(colorPiece: string): string;
        public getImageName(char: string): string;
        public isUpperCase(val: string): bool;
        public setScoreBoard(playerColor: string, activeColor: string, fen: string, pgn: string): void;
        public setupSignalR(game: Game, userId: number): void;
        public findSquare(x: number, y: number): string;
        public findPosition(position: number): any[];
        public squareToPosition(square): number;
        public convertLetterToFile(letter): number;
        public convertFileToLetter(file: number): string;
        public roundDown(val): number;
        public getScoreBoard(scoreBoardId: string): HTMLElement;
        public isThisAPawnPromotion(activeColor, matrix, startSquare, endSquare): bool;
        public showPawnPromotionDialog(id, activeColor, startSquare, endSquare): void;
    }
    class GameImage {
        public Image;
        public Width;
        public Height;
        public Url;
        public Rotation: number;
        public Shape: Shape;
        constructor(url: string, width: number, height: number);
        public rotate(rot: number): void;
        public load(): void;
        public getRoot(): string;
    }
}
