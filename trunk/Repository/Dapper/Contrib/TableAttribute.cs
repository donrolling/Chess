using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Dapper.Contrib {
	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : Attribute {
		public TableAttribute(string tableName) {
			Name = tableName;
		}
		public string Name { get; private set; }
	}
}
