using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Statuses;
using Repository.Responses;

namespace Repository {
	public class ActionResponse {
		public int Id { get; set; }
		public ActionType ActionType { get; set; }
		public Status Status { get; set; }
		public StatusDetail StatusDetail { get; set; }
		public string ErrorMessage { get; set; }
		public string SQL { get; set; }
		public dynamic Params { get; set; }

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail) {
			return GetActionResponse(actionType, status, 0, string.Empty, string.Empty, null);
		}

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail, int id) {
			return GetActionResponse(actionType, status, statusDetail, id, string.Empty, string.Empty, null);
		}

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail, int id, string errorMessage) {
			return GetActionResponse(actionType, status, statusDetail, id, errorMessage, string.Empty, null);
		}

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail, int id, string errorMessage, string sql) {
			return GetActionResponse(actionType, status, statusDetail, id, errorMessage, sql, null);
		}

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail, string errorMessage, string sql, dynamic parameters) {
			return GetActionResponse(actionType, status, statusDetail, 0, errorMessage, sql, parameters);
		}

		public static ActionResponse GetActionResponse(ActionType actionType, Status status, StatusDetail statusDetail, int id, string errorMessage, string sql, dynamic parameters) {
			return new ActionResponse { 
				ActionType = actionType,
				Status = status,
				StatusDetail = statusDetail,
				Id = id,
				ErrorMessage = errorMessage,
				SQL = sql,
				Params = parameters
			};
		}
	}
}
