using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;

namespace Repository.Interface {
	public interface IRepository {
		T Get<T>(long id, string schema = null) where T : class;
		ActionResponse Insert<T>(T entity) where T : class;
		ActionResponse Update<T>(T entity) where T : class;
		ActionResponse Delete<T>(long entityId) where T : class;
		ActionResponse Delete<T>(T entityId) where T : class;
		ActionResponse Delete<T>(List<T> entityIdList) where T : class;
		ActionResponse Execute(string sql, dynamic param);
	}
}
