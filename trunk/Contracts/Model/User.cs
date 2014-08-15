using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model {
	public class User {
		[Key]
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public string Salt { get; set; }
		public string Password { get; set; }
		public int? Rating { get; set; }
	}
}
