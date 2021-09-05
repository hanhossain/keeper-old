using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Core.Models;

namespace Keeper.Core.Services
{
    public interface IPlayerService
    {
        Task<List<Player>> GetPlayersAsync();

        Task<Player> GetPlayerAsync(int playerId);
    }
}
