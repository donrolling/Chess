using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Service.Membership;
using Service.Services;
using Service.Utilities;
using Contracts.Model;
using Contracts.ViewModels;
using Service.Interfaces;
using Microsoft.AspNet.SignalR.Hubs;
using Autofac;

namespace WebSite.Hubs {
	[HubName("chessHub")]
	public class ChessHub : Hub {
		readonly ILifetimeScope _hubLifetimeScope;
		readonly SignalRClientRepository _clientRepository;		
		readonly IUserService _userService;
		readonly IGameService _gameService;
		readonly IChessBoardService _chessBoardService;

		public ChessHub(ILifetimeScope lifetimeScope){
			_clientRepository = SignalRClientRepository.GetInstance();
			
			// Create a lifetime scope for the hub.
			_hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

			// Resolve dependencies from the hub lifetime scope.
			_userService = _hubLifetimeScope.Resolve<IUserService>();
			_gameService = _hubLifetimeScope.Resolve<IGameService>();
			_chessBoardService = _hubLifetimeScope.Resolve<IChessBoardService>();
		}

		public override Task OnConnected() {
			var user = new ChessHubUser() {
				Id = Clients.Caller.userId,
				Username = Clients.Caller.emailAddress
			};
			_clientRepository.Add(user, Context.ConnectionId);
			return base.OnConnected();
		}
		public override Task OnDisconnected() {
			bool success = _clientRepository.Remove(Context.ConnectionId);
			return base.OnDisconnected();
		}

		private HttpContextBase GetContext(){
			return this.Context.Request.GetHttpContext();
		}

		public void Get(int id) {
			if (!isAuthorized(id, Clients.Caller.userId)) { return; }
			var chessBoardViewModel = _chessBoardService.GetOngoingGame(id);
			Clients.All.TalariusChess.Game.HubCom.displayGameStatus(chessBoardViewModel, true);
		}
		public void Move(int id, string activeColor, string startSquare, string endSquare) {
			if (!isAuthorized(id, Clients.Caller.userId)) { return; }
			// Call the broadcastMessage method to update clients.
			var chessBoardViewModel = _chessBoardService.PlayMove(id, activeColor, startSquare, endSquare);
			Clients.All.TalariusChess.Game.HubCom.displayGameStatus(chessBoardViewModel, false);
		}
		public void Promote(int id, string activeColor, string startSquare, string endSquare, string promoteToPiece) {
			if (!isAuthorized(id, Clients.Caller.userId)) { return; }
			var chessBoardViewModel = _chessBoardService.PlayMove(id, activeColor, startSquare, endSquare, promoteToPiece[0]);
			Clients.All.TalariusChess.Game.HubCom.displayGameStatus(chessBoardViewModel, false);
		}
		protected override void Dispose(bool disposing) {
			// Dipose the hub lifetime scope when the hub is disposed.
			if (disposing && _hubLifetimeScope != null){
				_hubLifetimeScope.Dispose();
			}
			base.Dispose(disposing);
		}

		private bool isAuthorized(int id, string callerUserId) {
			var userId = 0;
			int.TryParse(new SimpleAES().DecryptString(Clients.Caller.userId), out userId);
			if (userId == 0) { return false; }
			var user = _userService.Get(userId);
			var hasGame = _userService.HasGame(id, user.Id);
			if (hasGame) {
				return true;
			}
			return false;
		}
	}
}