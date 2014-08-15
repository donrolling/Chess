using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Dapper.Contrib {
	public interface IProxy	 {
		bool IsDirty { get; set; }
	}
}
