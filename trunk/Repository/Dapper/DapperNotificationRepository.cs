using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;
using Repository.Interface;

namespace Repository.Dapper {
	public class DapperNotificationRepository : DapperRepository, INotificationRepository {
		public DapperNotificationRepository(string connectionString) : base(connectionString) { }

	}
}
