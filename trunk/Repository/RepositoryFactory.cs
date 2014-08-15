using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Dapper;
using Repository.Interface;
using Repository;

namespace Repository {
	public static class RepositoryFactory {
		private static string _connectionString;
		public static string ConnectionString {
			get {
				if (_connectionString == null) {
					throw new Exception("Repository Factory must be initialized with a connection string on application start.");
				}
				return _connectionString;
			}
		}

		private static RepositoryType _repositoryType = RepositoryType.Invalid;
		public static RepositoryType RepositoryType {
			get {
				if (_repositoryType == RepositoryType.Invalid) {
					throw new Exception("RepositoryFactory.CreateRepository was passed an incompatible Repository Type.");
				}
				return _repositoryType;
			}
		}

		public static void Initialize(string connectionString, RepositoryType repositoryType) {
			if (repositoryType == RepositoryType.Invalid) {
				throw new Exception("RepositoryFactory.CreateRepository was passed an incompatible Repository Type.");
			}
			_repositoryType = repositoryType;

			if (connectionString == null) {
				throw new Exception("Repository Factory must be initialized with a connection string on application start.");
			}
			_connectionString = connectionString;
		}

		public static IRepository Repository() {
			switch (RepositoryType) {
				case RepositoryType.Dapper:
					return new DapperRepository(ConnectionString);
				default:
					throw new Exception("RepositoryFactory.CreateRepository was passed an incompatible Repository Type.");
					return null;
			}
		}
	}
}
