using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model {
	public class User_GameData {
		[Key]
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int CreatorUserId { get; set; }
		public int OpponentUserId { get; set; }
		public int? GameDataId { get; set; }
		public string CreatorPlayerColor { get; set; }
		public bool ActiveGame { get; set; }
		
		public GameData GameData { get; set; }

		public User CreatorUser { get; set; }
		public User OpponentUser { get; set; }
	}
}