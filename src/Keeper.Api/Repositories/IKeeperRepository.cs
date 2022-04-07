using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database.Models;

namespace Keeper.Api.Repositories;

public interface IKeeperRepository
{
    Task<List<SleeperPlayer>> GetPlayersAsync(string query, CancellationToken cancellationToken);

    Task<SleeperPlayer> GetPlayerAsync(string playerId, CancellationToken cancellationToken);

    Task<NflPlayer> GetPlayerSeasonStatisticsAsync(string playerId, int season, CancellationToken cancellationToken);
}
