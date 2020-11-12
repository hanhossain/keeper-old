using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.Core.Models;
using Keeper.Core.Sleeper;
using Nito.AsyncEx;

namespace Keeper.Core.Services
{
    public class PlayerService
    {
        private readonly AsyncLock _lock = new AsyncLock();

        private List<Player> _players;

        public async Task<List<Player>> GetPlayersAsync()
        {
            using var lease = await _lock.LockAsync();

            if (_players == null)
            {
                var validPositions = new HashSet<string> { "QB", "RB", "WR", "TE", "K", "DEF" };
                using var sleeper = new SleeperClient();
                var players = await sleeper.GetPlayersAsync();
                _players = players
                    .Values
                    .Where(x => validPositions.Contains(x.Position))
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .Select(x => new Player()
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName
                    })
                    .ToList();
            }

            return _players;
        }
    }
}
