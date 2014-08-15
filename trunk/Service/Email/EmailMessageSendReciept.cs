using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Email {
	public class EmailMessageSendReciept {
		public enum MessageType {
			Error = 0,
			Success = 1
		}

		private bool _isHtml = false;
		private bool _isHtmlList = true;

		private readonly List<string> _messages = new List<string>();

		private string _message = string.Empty;
		private bool _success = true;

		public EmailMessageSendReciept() {
		}

		public EmailMessageSendReciept(bool isHtml) {
			_isHtml = isHtml;
		}

		public EmailMessageSendReciept(bool isHtml, bool isHtmlList) {
			_isHtml = isHtml;
			_isHtmlList = isHtmlList;
			if (_isHtmlList) {
				_isHtml = true;
			}
		}

		public bool Success {
			get { return _success; }
			set { _success = value; }
		}

		public string Message {
			get {
				string msg = messageToString();
				return msg;
			}
		}
		public List<string> Messages {
			get {
				return _messages;
			}
		}

		public void AddMessage(List<string> messages, MessageType messageType) {
			foreach (string message in messages) {
				AddMessage(message, messageType);
			}
		}

		public void AddMessage(string message, MessageType messageType) {
			_messages.Add(message);
			switch (messageType) {
				case MessageType.Error:
					_success = false;
					break;
				case MessageType.Success: //never set success here because it is true by default and if any errors are added, then it should become and remain false.
					break;
			}
		}

		private string messageToString() {
			string retval = string.Empty;

			const string beginMessageListCharacter = "<ul>";
			const string endMessageListCharacter = "</ul>";

			string beginLineCharacter = string.Empty;
			if (_isHtml && _isHtmlList) {
				beginLineCharacter = "<li>";
			}

			string endLineCharacter;
			if (_isHtml) {
				if (_isHtmlList) {
					endLineCharacter = "</li>";
				} else {
					endLineCharacter = "<br />";
				}
			} else {
				endLineCharacter = Environment.NewLine;
			}

			int i = 1;
			foreach (string message in _messages) {
				if (_isHtml) {
					if (_isHtmlList) {
						if (i == 1) {
							retval += beginMessageListCharacter;
						}
						retval += beginLineCharacter;
					}
				}
				retval += message + endLineCharacter;
				if (_isHtml && _isHtmlList && i == _messages.Count) {
					retval += endMessageListCharacter;
				}
				i++;
			}

			return retval;
		}
	}

}
