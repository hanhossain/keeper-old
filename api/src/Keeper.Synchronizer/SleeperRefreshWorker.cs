using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Redis;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Keeper.Synchronizer
{
    // TODO: turn this into a timer based background task instead
    // TODO: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio#timed-background-tasks
    public class SleeperRefreshWorker : BackgroundService
    {
        public const string SleeperLastUpdatedKey = "SleeperLastUpdated";

        private readonly ILogger<SleeperRefreshWorker> _logger;
        private readonly ISleeperClient _sleeperClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRedisClient _redisClient;
        private readonly ActivitySource _activitySource;
        private readonly TimeSpan _sleeperUpdatePeriod = TimeSpan.FromDays(1);

        public SleeperRefreshWorker(ILogger<SleeperRefreshWorker> logger,
            ISleeperClient sleeperClient,
            IServiceProvider serviceProvider,
            IRedisClient redisClient,
            ActivitySource activitySource)
        {
            _logger = logger;
            _sleeperClient = sleeperClient;
            _serviceProvider = serviceProvider;
            _redisClient = redisClient;
            _activitySource = activitySource;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextUpdate = await UpdateSleeperPlayersAsync(stoppingToken);
                var delay = nextUpdate - DateTime.Now;
                _logger.LogInformation("Will sleep for {}", delay);
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task<DateTime> UpdateSleeperPlayersAsync(CancellationToken cancellationToken)
        {
            using var activity = _activitySource.StartActivity();
            await using var scope = _serviceProvider.CreateAsyncScope();

            var sleeperLastUpdated = await _redisClient.GetDateAsync(SleeperLastUpdatedKey) ?? DateTime.MinValue;
            var nextUpdate = sleeperLastUpdated.Add(_sleeperUpdatePeriod);

            _logger.LogInformation("Sleeper last updated at [{}]. Sleeper next update at [{}]", sleeperLastUpdated, nextUpdate);

            if (nextUpdate <= DateTime.Now)
            {
                await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var players = await _sleeperClient.GetPlayersAsync();
                var existingPlayers = await databaseContext
                    .SleeperPlayers
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

                var validPositions = new HashSet<string>() { "QB", "RB", "WR", "TE", "K", "DEF" };
                var invalidNames = new HashSet<string>() { "Duplicate Player", "Player Invalid" };
                
                foreach (var player in players.Values)
                {
                    if (validPositions.Contains(player.Position) && !invalidNames.Contains(player.FirstName))
                    {
                        if (!existingPlayers.TryGetValue(player.PlayerId, out var dbPlayer))
                        {
                            dbPlayer = new SleeperPlayer()
                            {
                                Id = player.PlayerId
                            };

                            databaseContext.SleeperPlayers.Add(dbPlayer);
                        }

                        dbPlayer.Active = player.Active;
                        dbPlayer.Position = player.Position;
                        dbPlayer.Status = player.Status;
                        dbPlayer.Team = player.Team;
                        dbPlayer.FirstName = player.FirstName;
                        dbPlayer.LastName = player.LastName;
                        dbPlayer.FullName = $"{player.FirstName} {player.LastName}";                        
                    }
                    else if (existingPlayers.TryGetValue(player.PlayerId, out var playerToRemove))
                    {
                        databaseContext.Remove(playerToRemove);
                        existingPlayers.Remove(player.PlayerId);
                    }
                }

                await databaseContext.SaveChangesAsync(cancellationToken);
                var lastUpdated = DateTime.Now;
                nextUpdate = lastUpdated.Add(_sleeperUpdatePeriod);

                await _redisClient.SetAsync(SleeperLastUpdatedKey, lastUpdated);
                _logger.LogInformation("Added/updated all sleeper players in database. Next update will be at [{}]", nextUpdate);
            }

            return nextUpdate;
        }
    }
}
