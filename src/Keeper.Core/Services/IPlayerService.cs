using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Core.Models;
using Keeper.Core.Nfl.Statistics;

namespace Keeper.Core.Services
{
    public interface IPlayerService
    {
        Task<List<Player>> GetPlayersAsync();

        Task<Player> GetPlayerAsync(int playerId);

        Task<Dictionary<int, PlayerStatistics>> GetPlayerStatistics(int playerId, int season);

        Task<List<PlayerMatchup>> GetPlayerMatchups(int playerId, int season, int week);
    }
}
