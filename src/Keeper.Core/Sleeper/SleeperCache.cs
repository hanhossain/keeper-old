using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Keeper.Core.Sleeper
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
            await using var dbContext = new DatabaseContext();
            return await dbContext.SleeperPlayers
                .OrderBy(x => x.LastName)
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }
    }
}

