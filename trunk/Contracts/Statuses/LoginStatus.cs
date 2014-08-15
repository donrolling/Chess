using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Statuses {
	public class LoginStatus {
		public enum LoginStatuses {
			Success,
			Failure,
			Error,
			UsernameNotFound
		}

		public LoginStatuses Status { get; set; }
		public long Id { get; set; }
		public string Message { get; set; }

		public static LoginStatus GetLoginStatus(LoginStatuses loginStatus){
			return GetLoginStatus(loginStatus, 0);
		}
		public static LoginStatus GetLoginStatus(LoginStatuses loginStatus, long id){
			var status = new LoginStatus { Status = loginStatus };
			
			var message = string.Empty;
			switch (loginStatus) {
				case LoginStatuses.Success:
					message = "";
					break;
				case LoginStatuses.Failure:
					message = "Username or password is not correct.";
					break;
				case LoginStatuses.Error:
					message = "";
					break;
				case LoginStatuses.UsernameNotFound:
					message = "Username or password is not correct.";
					break;
			}
			status.Message = message;

			return status;
		}
	}
}