using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Contracts.Model;

namespace WebSite.Hubs {
	public class SignalRClientRepository {
		private static ICollection<ChessHubUser> _connectedUsers;
		private static Dictionary<string, string> _mappings;
		private static SignalRClientRepository _instance = null;
		private static readonly int max_random = 3;
		
		private SignalRClientRepository() {
			_connectedUsers = new List<ChessHubUser>();
			_mappings = new Dictionary<string, string>();
		}
		
		public static SignalRClientRepository GetInstance() {
			if (_instance == null) {
				_instance = new SignalRClientRepository();
			}
			return _instance;
		}

		public IQueryable<ChessHubUser> Users { get { return _connectedUsers.AsQueryable(); } }

		public ChessHubUser GetUserByConnectionId(string connectionId) {
			string userId = getUserIdByConnectionId(connectionId);
			if (userId != null) {
				var user = Users.Where(u => u.Id == userId).FirstOrDefault();
				if (user != null) {
					return user;
				}
			}
			return null;
		}
		public void Add(ChessHubUser user, string connectionId) {
			//make sure all params are valid
			if (!string.IsNullOrEmpty(connectionId) && !string.IsNullOrEmpty(user.Id) && !string.IsNullOrEmpty(user.Id)) {
				//make sure nobody already has this connection id
				if (string.IsNullOrEmpty(getUserIdByConnectionId(connectionId))) {
					//only add the user if he's new
					if (!_connectedUsers.Any(a => a.Username == user.Username)) {
						_connectedUsers.Add(user);
					}
					//add the mapping either way, because a user can be mapped to multiple connections
					_mappings.Add(connectionId, user.Id);	
				}
			}
		}
		public void Remove(ChessHubUser user) {
			if (_connectedUsers.Any(a => a.Username == user.Username)) {
				_connectedUsers.Remove(user);
			}
		}
		public bool Remove(string connectionId) {
			var user = GetUserByConnectionId(connectionId);
			if (user != null) {
				Remove(user);
				return true;
			}
			return false;
		}
		
		private string getUserIdByConnectionId(string connectionId) {
			string userId = null;
			_mappings.TryGetValue(connectionId, out userId);
			return userId;
		}
	}
}