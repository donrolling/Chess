using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Contracts.Statuses;
using Dapper;
using Repository.Dapper.Contrib;
using Repository.Interface;
using Repository.Responses;

namespace Repository.Dapper {
	public class DapperRepository : IRepository {
		private static string _connectionString;
		public static string ConnectionString {
			get {
				if (_connectionString == null) {
					throw new Exception("Repository must be initialized with a connection string.");
				}
				return _connectionString;
			}
		}

		public DapperRepository(string connectionString) {
			_connectionString = connectionString;
		}

		public T Get<T>(long id, string schema = null) where T : class {
			T entity;
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				entity = connection.Get<T>(id, schema: schema);
			}
			return entity;
		}

		public ActionResponse Insert<T>(T entity) where T : class {
			var result = 0;

			setAuditFields(entity, true);

			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				using (var connection = new SqlConnection(ConnectionString)) {
					connection.Open();
					using (var transaction = connection.BeginTransaction()) {
						result = connection.Insert<T>(entity, transaction);
						transaction.Commit();
					}
				}
			} catch (Exception ex) {
				status = Status.Failure;
				errorMessage = ex.Message;
			}
			return ActionResponse.GetActionResponse(ActionType.Create, status, StatusDetail.New, result, errorMessage);
		}

		public ActionResponse Update<T>(T entity) where T : class {
			setAuditFields(entity, false);

			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				using (var connection = new SqlConnection(ConnectionString)) {
					connection.Open();
					connection.Update<T>(entity);
				}
			} catch (Exception ex) {
				status = Status.Failure;
				errorMessage = ex.Message;
			}
			return ActionResponse.GetActionResponse(ActionType.Update, status, StatusDetail.Unknown, getId(entity), errorMessage);
		}

		public ActionResponse Delete<T>(long entityId) where T : class {
			T result = this.Get<T>(entityId);
			return this.Delete<T>(result);
		}

		public ActionResponse Delete<T>(T entity) where T : class {
			if (entity == null) {
				return ActionResponse.GetActionResponse(ActionType.Delete, Status.ItemNotFound, StatusDetail.ItemNotFound, 0, "Cannot delete a null entity");
			}

			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				using (var connection = new SqlConnection(ConnectionString)) {
					connection.Open();
					var success = connection.Delete<T>(entity);
					if (!success) {
						status = Status.Failure;
					}
				}
			} catch (Exception ex) {
				status = Status.Failure;
				errorMessage = ex.Message;
			}
	
			return ActionResponse.GetActionResponse(ActionType.Delete, status, StatusDetail.Unknown, getId<T>(entity), errorMessage);
		}

		public ActionResponse Delete<T>(List<T> entityIdList) where T : class {
			if (entityIdList == null) {
				return ActionResponse.GetActionResponse(ActionType.Delete, Status.ItemNotFound, StatusDetail.ItemNotFound, 0, "Cannot delete a list of null entities");
			}
			
			var status = Status.Success;
			var errorMessage = string.Empty;
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				foreach (var entity in entityIdList) {
					try {
						var success = connection.Delete<T>(entity);
						if (!success) {
							status = Status.Failure;
						}
					} catch (Exception ex) {
						status = Status.Failure;
						errorMessage = ex.Message;
					}
				}
			}

			return ActionResponse.GetActionResponse(ActionType.Delete, status, StatusDetail.Unknown, 0, errorMessage);
		}

		public ActionResponse Execute(string sql, dynamic param) {
			var status = Status.Success;
			var errorMessage = string.Empty;
			try {
				using (var connection = new SqlConnection(ConnectionString)) {
					connection.Open();
					using (var transaction = connection.BeginTransaction()) {
						var result = SqlMapper.Execute(connection, sql, param, transaction);
						transaction.Commit();
					}
				}
			} catch (Exception ex) {
				status = Status.Failure;
				errorMessage = ex.Message;
			}

			return ActionResponse.GetActionResponse(ActionType.Execute, status, StatusDetail.Unknown, 0, errorMessage, sql, param);
		}

		protected IEnumerable<T> Query<T>(string sql, dynamic param) where T : class {
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				return SqlMapper.Query<T>(connection, sql, param);
			}
		}
		protected IEnumerable<R> Query<T, U, R>(string sql, Func<T, U, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where R : class {
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				return SqlMapper.Query<T, U, R>(connection, sql, func, param, splitOn: splitOn);
			}
		}
		protected IEnumerable<R> Query<T, U, V, R>(string sql, Func<T, U, V, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where R : class
			where V : class {
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				return SqlMapper.Query<T, U, V, R>(connection, sql, func, param, splitOn: splitOn);
			}
		}
		protected IEnumerable<R> Query<T, U, V, W, R>(string sql, Func<T, U, V, W, R> func, dynamic param, string splitOn = null)
			where T : class
			where U : class
			where V : class
			where W : class
			where R : class {
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				return SqlMapper.Query<T, U, V, W, R>(connection, sql, func, param, splitOn: splitOn);
			}
		}

		protected string formatError(Exception ex) {
			var msg = ex.Message.ToString();
			if (ex.InnerException != null) {
				msg += string.Concat("\nInner Exception: ", ex.InnerException.Message.ToString());
			}
			return msg;
		}

		public static long GetQueryTotal(string baseQuery, DynamicParameters dynamicData) {
			var countQuery = GetCountQuery(baseQuery);
			long total = 0;
			using (var connection = new SqlConnection(ConnectionString)) {
				connection.Open();
				total = SqlMapper.Query<long>(connection, countQuery, dynamicData).First();
			}
			return total;
		}

		public static Tuple<long, long> GetRowStartAndRowEnd(long pageStart, long pageSize) {
			var rowEnd = pageStart + pageSize;
			return Tuple.Create<long, long>(pageStart, rowEnd);
		}

		public static string WrapQueryInRowNumberQuery(string baseQuery, string keyName) {
			var frontAndBack = GetQueryFrontAndBack(baseQuery);
			//this assumes that the last FROM in your query is the one with the joins and stuff.
			//it helps if you have subselects, but could get hosed in certain circumstances
			var realQuery = string.Concat(
								"WITH result AS(",
								frontAndBack.Item1,
								", ROW_NUMBER() OVER (ORDER BY ",
								keyName,
								") AS RowNumber ",
								frontAndBack.Item2,
								") SELECT * FROM result WHERE RowNumber BETWEEN @rowStart and @rowEnd"
							);
			return realQuery;
		}

		public static string GetCountQuery(string query) {
			var frontAndBack = GetQueryFrontAndBack(query);
			var countQuery = string.Concat("select count(*) as Total ", frontAndBack.Item2);
			return countQuery;
		}

		public static Tuple<string, string> GetQueryFrontAndBack(string baseQuery) {
			var lastFrom = baseQuery.LastIndexOf("from", StringComparison.CurrentCultureIgnoreCase);
			var frontPart = baseQuery.Substring(0, lastFrom);
			var backPart = baseQuery.Substring(lastFrom, baseQuery.Length - lastFrom);
			return Tuple.Create<string, string>(frontPart, backPart);
		}

		public static string PrepareForLikeQuery(string searchText, bool searchLeftSide, bool searchRightSide) {
			if (string.IsNullOrEmpty(searchText)) {
				return searchText;
			}

			var value = searchText.Replace("%", "[%]").Replace("[", "[[]").Replace("]", "[]]");

			if (searchLeftSide) {
				value = "%" + value;
			}
			if (searchRightSide) {
				value += "%";
			}

			return value;
		}

		protected void setAuditFields<T>(T entity, bool isNew) where T : class {
			if (isNew) {
				var creationDate = entity.GetType().GetProperty("CreatedDate", BindingFlags.Public | BindingFlags.Instance);
				if (creationDate != null && creationDate.CanWrite) {
					creationDate.SetValue(entity, DateTime.Now, null);
				}
			} else {
				var updatedDate = entity.GetType().GetProperty("UpdatedDate", BindingFlags.Public | BindingFlags.Instance);
				if (updatedDate != null && updatedDate.CanWrite) {
					updatedDate.SetValue(entity, DateTime.Now, null);
				}
			}
		}

		private int getId<T>(T entity) where T : class {
			try {
				var value = (int)typeof(T).GetProperty("Id").GetValue(entity, null);
				return value;
			} catch (Exception) {
				return 0;
			}
		}
	}
}