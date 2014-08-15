using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contracts.Statuses {
	public enum Status {
		Success,
		Failure,
		ItemNotFound
	}

	public enum StatusDetail {
		New,
		Duplicate,
		Error,
		ItemNotFound,
		Unknown
	}
}