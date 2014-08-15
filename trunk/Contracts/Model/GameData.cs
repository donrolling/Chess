using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model {
	public class GameData {
		[Key]
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string FEN { get; set; }
		public string PGN { get; set; }
		public string Result { get; set; }
	}
}
