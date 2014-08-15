using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTO {
	public class DisplayUser {
		[Key]
		public int Id { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public int? Rating { get; set; }
	}
}
