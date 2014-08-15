using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository {
	public class RepositoryActionResponse {
		public int Id { get; set; }
		
		private bool _success = true;
		public bool Success {
			get { return _success; }
			set { _success = value; }
		}

		private string _errorMessage;
		public string ErrorMessage {
			get {
				return _errorMessage;
			}
			set {
				_success = false;
				_errorMessage = value;
			}
		}
	}
}
