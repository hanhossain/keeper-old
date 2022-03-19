using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.Core.Database.Models;
using Keeper.Core.Sleeper;
using Microsoft.EntityFrameworkCore;

namespace Keeper.Core.Database
{
    public class SleeperCache : ISleeperCache
    {
        private readonly ISleeperClient _sleeperClient;

        public SleeperCache(ISleeperClient sleeperClient)
        {
            _sleeperClient = sleeperClient;
        }

        public async Task RefreshPlayersAsync()
        {
            var sleeperPlayers = await _sleeperClient.GetPlayersAsync();

            await using var dbContext = new DatabaseContext();

            foreach (var sleeperPlayer in sleeperPlayers.Values)
            {
                bool playerExists = await dbContext.SleeperPlayers.AnyAsync(x => x.Id == sleeperPlayer.PlayerId);

                var dbPlayer = new SleeperPlayer()
                {
                    Active = sleeperPlayer.Active,
                    FirstName = sleeperPlayer.FirstName,
                    LastName = sleeperPlayer.LastName,
                    FullName = $"{sleeperPlayer.FirstName} {sleeperPlayer.LastName}",
                    Id = sleeperPlayer.PlayerId,
                    Position = sleeperPlayer.Position,
                    Status = sleeperPlayer.Status,
                    Team = sleeperPlayer.Team
                };

                if (playerExists)
                {
                    dbContext.SleeperPlayers.Update(dbPlayer);
                }
                else
                {
                    dbContext.SleeperPlayers.Add(dbPlayer);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<SleeperPlayer>> GetPlayersAsync()
        {
            var positions = new HashSet<string>() { "QB", "RB", "WR", "TE", "K", "DEF" };

            await using var dbContext = new DatabaseContext();
            return await dbContext.SleeperPlayers
                .Where(x => positions.Contains(x.Position))
                .Where(x => x.Active)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }
    }
}

