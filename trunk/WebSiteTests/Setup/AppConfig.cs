using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Moq;
using Repository;
using Repository.Dapper;
using Repository.Interface;
using Service.Interfaces;
using Service.Membership;
using Service.Services;
using Subtext.TestLibrary;

namespace WebSiteTests.Tests.Setup {
	internal class AppConfig {
		public static void RegisterTestApp(){
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
			
			var _httpSimulator = new HttpSimulator().SimulateRequest();
			RegisterInjection(connectionString, smtpServer, adminEmail, emailPath, environment);
		}

		public static void RegisterInjection(string connectionString, string smtpServer, string adminEmail, string emailPath, string environment){
			//setup injection
			var builder = new ContainerBuilder();

			//register controllers
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			//setup repos
			builder.RegisterType<DapperUserRepository>().As<IUserRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);
			builder.RegisterType<DapperGameDataRepository>().As<IGameDataRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);
			builder.RegisterType<DapperNotificationRepository>().As<INotificationRepository>().InstancePerLifetimeScope().WithParameter("connectionString", connectionString);

			//set services
			builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
			builder.RegisterType<GameService>().As<IGameService>().InstancePerLifetimeScope();
			builder.RegisterType<ChessBoardService>().As<IChessBoardService>().InstancePerLifetimeScope();
			builder.RegisterType<NotificationService>()
					.As<INotificationService>()
					.InstancePerDependency()
					.WithParameter("smtpServer", smtpServer)
					.WithParameter("adminEmail", adminEmail)
					.WithParameter("emailPath", emailPath)
					.WithParameter("environment", environment);

			//setup DependencyResolver
			var container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}