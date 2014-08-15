using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Model;

namespace Repository.Interface {
	public interface IGameDataRepository : IRepository {
		User_GameData GetGameRequest(int userGameDataId);

		User_GameData GetByGameId(int gameId);
	}
}
