using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Repository;
using Repository.Dapper;
using Repository.Interface;
using Service.Email;
using Service.Interfaces;
using Service.Membership;
using Service.Services;
using WebSite.Hubs;

namespace WebSite {
	public class AppConfig {
		public static void RegisterApp() {
			//get Connection String
			var connectionString = ConfigurationManager.ConnectionStrings["TalariusChess"].ConnectionString;
			if (string.IsNullOrEmpty(connectionString)) {
				throw new Exception("Empty Connection String.");
			}
			
			var environment = ConfigurationManager.AppSettings["Environment"];
			var adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];			
			//get notification service info
			var smtpServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
			if (string.IsNullOrEmpty(smtpServer)) {
				throw new Exception("Web.Config value for SMTPServer is null.");
			}
			if (string.IsNullOrEmpty(adminEmail)) {
				throw new Exception("Web.Config value for AdminEmail is null.");
			}
			var emailPath = System.Configuration.ConfigurationManager.AppSettings["EmailPath"];
			RegisterInjection(connectionString, smtpServer, adminEmail, emailPath, environment);
					
			//Initialize UserTracker
			var userService = (IUserService)DependencyResolver.Current.GetService(typeof(IUserService));

			var autoLoginUser = false;
			bool.TryParse(ConfigurationManager.AppSettings["AutoLoginUser"], out autoLoginUser);
			if (autoLoginUser) {
				userService.AutoLoginUser(adminEmail, environment);
			}
			
			RouteTable.Routes.MapHubs();
		}

		public static void RegisterInjection(string connectionString, string smtpServer, string adminEmail, string emailPath, string environment){
			//setup injection
			var builder = new ContainerBuilder();
			builder.RegisterControllers(typeof(HttpApplication).Assembly);
			// Register the SignalR hubs.
			builder.RegisterHubs(Assembly.GetExecutingAssembly());
			
			//set repository
			builder.RegisterType<DapperUserRepository>().As<IUserRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);
			builder.RegisterType<DapperGameDataRepository>().As<IGameDataRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);
			builder.RegisterType<DapperNotificationRepository>().As<INotificationRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);

			//set services
			builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
			builder.RegisterType<GameService>().As<IGameService>().InstancePerDependency();
			builder.RegisterType<ChessBoardService>().As<IChessBoardService>().InstancePerDependency();
			builder.RegisterType<NotificationService>()
					.As<INotificationService>()
					.InstancePerDependency()
					.WithParameter("smtpServer", smtpServer)
					.WithParameter("adminEmail", adminEmail)
					.WithParameter("emailPath", emailPath)
					.WithParameter("environment", environment);

			//setup DependencyResolver
			var container = builder.Build();
			//set mvc resolver
			var mvcResolver = new Autofac.Integration.Mvc.AutofacDependencyResolver(container);
			DependencyResolver.SetResolver(mvcResolver);

			// Configure SignalR with the dependency resolver.
			var signalRResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);
			GlobalHost.DependencyResolver = signalRResolver;
		}
	}
}