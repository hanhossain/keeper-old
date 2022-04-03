using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Keeper.Synchronizer;

public class PlayerLinkWorker : BackgroundService
{
    private const string PlayersLastLinkedKey = "PlayersLastLinked";

    private readonly ILogger<PlayerLinkWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ActivitySource _activitySource;
    private readonly IRedisClient _redisClient;
    private readonly TimeSpan _updatePeriod = TimeSpan.FromDays(1);

    public PlayerLinkWorker(ILogger<PlayerLinkWorker> logger, IServiceProvider serviceProvider, ActivitySource activitySource, IRedisClient redisClient)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _activitySource = activitySource;
        _redisClient = redisClient;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextUpdate = await LinkPlayersAsync(stoppingToken);
            var delay = nextUpdate - DateTime.Now;
            _logger.LogInformation("Will sleep for {}", delay);
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task<bool> DependenciesReadyAsync()
    {
        var sleeperLastUpdated = await _redisClient.GetDateAsync(SleeperRefreshWorker.SleeperLastUpdatedKey);

        if (sleeperLastUpdated == null)
        {
            _logger.LogInformation("Sleeper not ready yet");
            return false;
        }

        var nflFantasyLastUpdated = await _redisClient.GetDateAsync(NflFantasyRefreshWorker.NflFantasyLastUpdatedKey);

        if (nflFantasyLastUpdated == null)
        {
            _logger.LogInformation("Nfl Fantasy not ready yet");
            return false;
        }

        return true;
    }

    private async Task<DateTime> LinkPlayersAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();

        var playersLastLinked = await _redisClient.GetDateAsync(PlayersLastLinkedKey);

        if (playersLastLinked == null)
        {
            _logger.LogInformation("Players not previously linked");

            if (!await DependenciesReadyAsync())
            {
                var nextCheck = DateTime.Now.AddSeconds(10);
                _logger.LogInformation("Dependencies not ready. Next update will be at [{}]", nextCheck);
                return nextCheck;
            }
        }

        var nextUpdate = (playersLastLinked ?? DateTime.MinValue).Add(_updatePeriod);

        if (nextUpdate <= DateTime.Now)
        {
            await LinkPlayersWithNamePositionTeamAsync(cancellationToken);
            await LinkPlayersWithNamePositionAsync(cancellationToken);
            await LinkPlayersWithNameTeamAsync(cancellationToken);
            await LinkPlayersWithNameAsync(cancellationToken);

            var lastUpdated = DateTime.Now;
            nextUpdate = lastUpdated.Add(_updatePeriod);
            await _redisClient.SetAsync(PlayersLastLinkedKey, lastUpdated);
            _logger.LogInformation("Linked players. Next update will be at [{}]", nextUpdate);
        }

        return nextUpdate;
    }

    private async Task LinkPlayersWithNamePositionTeamAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var players = await databaseContext.SleeperPlayers
            .Where(x => x.NflId == null && x.Active)
            .Join(
                RemainingNflPlayersQuery(databaseContext),
                x => new { Name = x.FullName, x.Position, x.Team },
                x => new { x.Name, x.Position, x.Team },
                (sleeper, nfl) => new { Sleeper = sleeper, Nfl = nfl })
            .ToListAsync(cancellationToken);
        var groupedPlayers = players
            .GroupBy(x => (x.Sleeper.FullName, x.Sleeper.Position, x.Sleeper.Team))
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var player in groupedPlayers.Values)
        {
            player.Sleeper.NflId = player.Nfl.Id;
        }

        await databaseContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Finished linking {} players with name, position, and team", groupedPlayers.Count);
    }

    private async Task LinkPlayersWithNamePositionAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var players = await databaseContext.SleeperPlayers
            .Where(x => x.NflId == null && x.Active)
            .Join(
                RemainingNflPlayersQuery(databaseContext),
                x => new { Name = x.FullName, x.Position },
                x => new { x.Name, x.Position },
                (sleeper, nfl) => new { Sleeper = sleeper, Nfl = nfl })
            .ToListAsync(cancellationToken);
        var groupedPlayers = players
            .GroupBy(x => (x.Sleeper.FullName, x.Sleeper.Position))
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var player in groupedPlayers.Values)
        {
            player.Sleeper.NflId = player.Nfl.Id;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Finished linking {} players with name and position", groupedPlayers.Count);
    }

    private async Task LinkPlayersWithNameTeamAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var players = await databaseContext.SleeperPlayers
            .Where(x => x.NflId == null && x.Active)
            .Join(
                RemainingNflPlayersQuery(databaseContext),
                x => new { Name = x.FullName, x.Team },
                x => new { x.Name, x.Team },
                (sleeper, nfl) => new { Sleeper = sleeper, Nfl = nfl })
            .ToListAsync(cancellationToken);
        var groupedPlayers = players
            .GroupBy(x => (x.Sleeper.FullName, x.Sleeper.Team))
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var player in groupedPlayers.Values)
        {
            player.Sleeper.NflId = player.Nfl.Id;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Finished linking {} players with name and team", groupedPlayers.Count);
    }

    private async Task LinkPlayersWithNameAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceProvider.CreateAsyncScope();
        await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var players = await databaseContext.SleeperPlayers
            .Where(x => x.NflId == null && x.Active)
            .Join(
                RemainingNflPlayersQuery(databaseContext),
                x => x.FullName,
                x => x.Name,
                (sleeper, nfl) => new { Sleeper = sleeper, Nfl = nfl })
            .ToListAsync(cancellationToken);
        var groupedPlayers = players
            .GroupBy(x => x.Sleeper.FullName)
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.First());

        foreach (var player in groupedPlayers.Values)
        {
            player.Sleeper.NflId = player.Nfl.Id;
        }
        
        await databaseContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Finished linking {} players with name", groupedPlayers.Count);
    }

    private IQueryable<NflPlayer> RemainingNflPlayersQuery(DatabaseContext databaseContext)
    {
        return from n in databaseContext.NflPlayers
            join s in databaseContext.SleeperPlayers
                on n.Id equals s.NflId into grouping
            from s in grouping.DefaultIfEmpty()
            where s.NflId == null
            select n;
    }
}
