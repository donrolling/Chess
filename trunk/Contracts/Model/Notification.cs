using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model {
	public class Notification {		
		[Key]
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int UserId { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public bool Read { get; set; }
		public bool Deleted { get; set; }
	}
}