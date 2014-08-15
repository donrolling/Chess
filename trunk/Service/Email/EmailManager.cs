using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.IO;

namespace Service.Email {
	public class EmailManager {
		public string SmtpServer { get; private set; }
		public string AdminEmail { get; private set; }
		public string EmailPath { get; private set; }
		public string Environment { get; private set; }

		public EmailManager(string smtpServer, string adminEmail, string emailPath, string environment) {
			this.SmtpServer = smtpServer;
			this.AdminEmail = adminEmail;
			this.Environment = environment;
			this.EmailPath = emailPath;
		}

		public EmailMessageSendReciept SendEmail(string body, string subject, string from, string to, string cc, string bcc, bool isBodyHtml) {
			return SendEmail(body, subject, from, to, cc, bcc, isBodyHtml, null);
		}
		public EmailMessageSendReciept SendEmail(string body, string subject, string from, string to, string cc, string bcc, bool isBodyHtml, List<Attachment> attachments) {
			var emailMessage = Messages.GetMailMessage(body, subject, from, to, cc, bcc, isBodyHtml, attachments);
			return SendEmail(emailMessage);
		}

		public EmailMessageSendReciept SendEmail(MailMessage mailMessage) {
			var emailMessageSendReciept = Messages.ValidateInputs(mailMessage);

			if (string.IsNullOrEmpty(SmtpServer)) {
				emailMessageSendReciept.Messages.Add("SMTP Server value is empty.");
			}

			if (emailMessageSendReciept.Success) {
				var emailSender = SmtpClientFactory.Create(SmtpServer, Environment, EmailPath);
				try {
					emailSender.Send(mailMessage);
				} catch (Exception ex) {
					emailMessageSendReciept.AddMessage("Exception: " + ex.Message, EmailMessageSendReciept.MessageType.Error);
					if (ex.InnerException != null && ex.InnerException.Message != null) {
						emailMessageSendReciept.AddMessage("Inner Exception: " + ex.InnerException.Message, EmailMessageSendReciept.MessageType.Error);
					}
					emailMessageSendReciept.AddMessage("Subject = " + mailMessage.Subject, EmailMessageSendReciept.MessageType.Error);
					emailMessageSendReciept.AddMessage("From Address = " + mailMessage.From, EmailMessageSendReciept.MessageType.Error);
					emailMessageSendReciept.AddMessage("Smtp Server = " + SmtpServer, EmailMessageSendReciept.MessageType.Error);
					emailMessageSendReciept.AddMessage("Is Html = " + mailMessage.IsBodyHtml, EmailMessageSendReciept.MessageType.Error);
				}
			} else {
				emailMessageSendReciept.AddMessage("Subject = " + mailMessage.Subject, EmailMessageSendReciept.MessageType.Error);
				emailMessageSendReciept.AddMessage("From Address = " + mailMessage.From, EmailMessageSendReciept.MessageType.Error);
				emailMessageSendReciept.AddMessage("Smtp Server = " + SmtpServer, EmailMessageSendReciept.MessageType.Error);
				emailMessageSendReciept.AddMessage("Is Html = " + mailMessage.IsBodyHtml, EmailMessageSendReciept.MessageType.Error);
			}

			return emailMessageSendReciept;
		}
	}

	public class SmtpClientFactory {
		public static SmtpClient Create(string smtpServer, string environment, string emailPath) {
			var smtpClient = new SmtpClient();
			if (environment == "Local") {
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
				if (string.IsNullOrEmpty(emailPath)) {
					throw new Exception("EmailPath value may not be empty in the local environment.");
				}
				var emailPickupDirectory = emailPath.Contains(':') ? emailPath : HostingEnvironment.MapPath(emailPath);
				if (!Directory.Exists(emailPickupDirectory)) {
					Directory.CreateDirectory(emailPickupDirectory);
				}
				smtpClient.PickupDirectoryLocation = emailPickupDirectory;
			} else {
				smtpClient.Host = smtpServer;
				smtpClient.Port = 25;
			}
			return smtpClient;
		}
	}
}
