using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Model {
	public class BoardHistory {
		public string Position { get; set; }
		public string CastlingAvailability { get; set; }
		public string EnPassantTargetSquare { get; set; }
	}
}
