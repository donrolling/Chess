using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Service.Email {
	public static class Messages {
		public static MailMessage ExtendChallengeMessage(string from, string to, string challengersDisplayName, string acceptChallengeUrl) {
			var subject = "Talarius Chess - New Game Challenge!";
			var body = "@@ChallengersDisplayName@@ has challenged you to a game of chess on the Talarius Chess website. <a href='AcceptChallengeUrl'>Click here</a> to accept the challenge.".Replace("@@ChallengersDisplayName@@", challengersDisplayName).Replace("@@AcceptChallengeUrl@@", acceptChallengeUrl);
			var isBodyHtml = true;

			var emailMessage = GetMailMessage(body, subject, from, to, null, null, isBodyHtml);

			return emailMessage;
		}

		public static MailMessage GetMailMessage(string body, string subject, string from, string to, string cc, string bcc, bool isBodyHtml) {
			return GetMailMessage(body, subject, from, to, cc, bcc, isBodyHtml, null);
		}
		public static MailMessage GetMailMessage(string body, string subject, string from, string to, string cc, string bcc, bool isBodyHtml, List<Attachment> attachments) {
			var emailMessage = new MailMessage {
				Body = body,
				Subject = subject,
				IsBodyHtml = isBodyHtml
			};

			emailMessage.From = new MailAddress(from);
			prepareAddresses(to).Select(a => a).ToList().ForEach(a => emailMessage.To.Add(a));
			if (!string.IsNullOrEmpty(cc)) { prepareAddresses(cc).Select(a => a).ToList().ForEach(a => emailMessage.CC.Add(a)); }
			if (!string.IsNullOrEmpty(bcc)) { prepareAddresses(bcc).Select(a => a).ToList().ForEach(a => emailMessage.Bcc.Add(a)); }

			return emailMessage;
		}

		private static MailAddressCollection prepareAddresses(string addresses) {
			var mailAddressCollection = new MailAddressCollection();
			if (!string.IsNullOrEmpty(addresses)) {
				addresses = addresses.Replace(";", ",");
				if (addresses.Contains(",")) {
					List<string> addressList = new List<string>(addresses.Split(Convert.ToChar(",")));
					foreach (var address in addressList) {
						var mailAddress = new MailAddress(address);
						mailAddressCollection.Add(mailAddress);
					}
				} else {
					var mailAddress = new MailAddress(addresses);
					mailAddressCollection.Add(mailAddress);
				}
			}
			return mailAddressCollection;
		}

		public static EmailMessageSendReciept ValidateInputs(MailMessage mailMessage) {
			EmailMessageSendReciept emailMessageSendReciept = new EmailMessageSendReciept();
			if (string.IsNullOrEmpty(mailMessage.Subject)) {
				emailMessageSendReciept.AddMessage("Subject is empty.", EmailMessageSendReciept.MessageType.Error);
			}
			if (mailMessage.To.Count == 0) {
				emailMessageSendReciept.AddMessage("Email recipient is empty.", EmailMessageSendReciept.MessageType.Error);
			}
			if (string.IsNullOrEmpty(mailMessage.From.Address)) {
				emailMessageSendReciept.AddMessage("Email sender is empty.", EmailMessageSendReciept.MessageType.Error);
			}
			if (string.IsNullOrEmpty(mailMessage.Body)) {
				emailMessageSendReciept.AddMessage("Email message body is missing", EmailMessageSendReciept.MessageType.Error);
			}
			return emailMessageSendReciept;
		}
	}
}