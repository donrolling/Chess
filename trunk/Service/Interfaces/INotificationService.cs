using System;
using Repository;
namespace Service.Interfaces {
	public interface INotificationService {
		string BuildChallengeLink(Uri uri, int gameId, int opponentId);
		ActionResponse NotifyNewChallenge(string opponentEmail, string challengerDisplayName, Uri uri, int gameId, int opponentId);
	}
}
