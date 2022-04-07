using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Keeper.Api.Repositories;

public class KeeperRepository : IKeeperRepository
{
    private readonly DatabaseContext _databaseContext;

    public KeeperRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<List<SleeperPlayer>> GetPlayersAsync(string query, CancellationToken cancellationToken)
    {
        var playerQuery = _databaseContext.SleeperPlayers.Where(x => x.Active);

        if (!string.IsNullOrEmpty(query))
        {
            playerQuery = playerQuery.Where(x => x.FullName.Contains(query));
        }

        return await playerQuery.ToListAsync(cancellationToken);
    }

    public async Task<SleeperPlayer> GetPlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        return await _databaseContext.SleeperPlayers.FindAsync(new object[] { playerId }, cancellationToken);
    }

    public async Task<NflPlayer> GetPlayerSeasonStatisticsAsync(string playerId,int season, CancellationToken cancellationToken)
    {
        return await _databaseContext
            .NflPlayers
            .Include(x => x.PlayerStatistics.Where(ps => ps.Season == season))
            .Include(x => x.NflOffensiveStatistics.Where(os => os.Season == season))
            .Where(x => x.SleeperPlayer.Id == playerId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
