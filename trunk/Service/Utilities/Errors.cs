using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Service.Utilities {
	public static class Errors {
		public static Exception NoUnknownPlayers = new Exception("Can't create a new game with an unknown opponent!");
		public static Exception UnknownPlayers = new Exception("Neither player can be empty or null when playing a chess game.");
		public static Exception EmptyGameData = new Exception("The game data cannot be empty or null when playing a chess game.");
	}
}