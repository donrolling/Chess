using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Contracts.Model;
using Chess.Model;

namespace Contracts.ViewModels {
	public class GameModel {
		public int Id { get; set; }
		public string EncryptedUserId { get; set; }

		public int Player1Id { get; set; }
		public string Player1Name { get; set; }
		public string Player1Color { get; set; }

		public int Player2Id { get; set; }
		public string Player2Name { get; set; }
		public string Player2Color { get; set; }

		public ChessBoardViewModel ChessBoardViewModel { get; set; }

		public DateTime CreatedDate { get; set; }

		public IEnumerable<SelectListItem> PotentialOpponents { get; set; }

		public string CurrentUserPlayerColor { get; set; }
	}
}