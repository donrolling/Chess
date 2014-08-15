using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using Omu.ValueInjecter;
using WebMatrix.WebData;
using Service.Services;
using Contracts.Model;
using Contracts.ViewModels;
using Contracts.Statuses;
using Service.Membership;
using Service.Interfaces;

namespace WebSite.Controllers {
	//[Authorize]
	public partial class AccountController : Controller {
		private IUserService _userService;

		public AccountController(IUserService userService) {
			this._userService = userService;
		}
		
		[AllowAnonymous]
		public virtual ActionResult Login(string returnUrl) {
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Login(LoginModel model, string returnUrl) {
			var loginStatus = _userService.Login(model.UserName, model.Password);

			switch (loginStatus.Status){
				case LoginStatus.LoginStatuses.Success:
					if (!string.IsNullOrEmpty(returnUrl)) {
						return Redirect(returnUrl);
					}
					return RedirectToAction("Index", "Home");
				case LoginStatus.LoginStatuses.Failure:
				case LoginStatus.LoginStatuses.Error:
				case LoginStatus.LoginStatuses.UsernameNotFound:
				default:
					ModelState.AddModelError("", loginStatus.Message);
					return View(model);
			}
		}

		public virtual ActionResult LogOff() {
			_userService.Logoff(this.HttpContext);

			return RedirectToAction("Index", "Home");
		}
		
		[AllowAnonymous]
		public virtual ActionResult Register() {
			return View();
		}
		
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public virtual ActionResult Register(RegisterModel model) {
			if(model.Password != model.ConfirmPassword){
				ModelState.AddModelError("Password", "Passwords must match.");
			}

			if (ModelState.IsValid) {
				var result = _userService.Create(model.DisplayName, model.Email, model.Password);
				switch (result.Status) {
					case Status.Success:
						_userService.Login(model.Email, model.Password);
						return RedirectToAction("Index", "Home");
					case Status.Failure:
					default:
						ViewBag.Error = result.ErrorMessage;
						return View(model);
				}
			}
			return View(model);
		}
	}
}
