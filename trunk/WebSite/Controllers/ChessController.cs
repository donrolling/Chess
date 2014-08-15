using Chess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Contracts.ViewModels;
using Contracts.Model;
using Service.Interfaces;

namespace WebSite.Controllers {
	public class ChessController : ApiController {
		IChessBoardService _chessBoardService;

		public ChessController(IChessBoardService chessBoardService) {
			this._chessBoardService = chessBoardService;
		}

		public ChessBoardViewModel Get(int id) { // GET api/chess/5
			ChessBoardViewModel chessBoardViewModel = _chessBoardService.GetOngoingGame(id);
			return chessBoardViewModel;
		}
		
		//GET api/chess/?id=5&activeColor=w&startSquare=1&endSquare=1
		public ChessBoardViewModel Get(int id, string activeColor, string startSquare, string endSquare) { 
			ChessBoardViewModel chessBoardViewModel = _chessBoardService.PlayMove(id, activeColor, startSquare, endSquare);
			return chessBoardViewModel;
		}
		
		//GET api/chess/?id=5&activeColor=w&startSquare=1&endSquare=1promoteToPiece=Q
		public ChessBoardViewModel Get(int id, string activeColor, string startSquare, string endSquare, string promoteToPiece) {
			ChessBoardViewModel chessBoardViewModel = _chessBoardService.PlayMove(id, activeColor, startSquare, endSquare, promoteToPiece[0]);
			return chessBoardViewModel;
		}
		
		public void Delete(int id) { // DELETE api/chess/5
		}
	}
}
