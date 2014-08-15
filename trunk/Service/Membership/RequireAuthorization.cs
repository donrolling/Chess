using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Contracts.Model;

namespace Service.Membership {
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class RequireAuthorization : AuthorizeAttribute {
		public override void OnAuthorization(AuthorizationContext filterContext){
			//User user = UserTracker.Current(filterContext.HttpContext);
			//if(user == null || user.Id == 0){
			//	filterContext.Result = new RedirectResult("~/Home/Index");
			//}
		}
	}
}