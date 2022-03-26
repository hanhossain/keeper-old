using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Keeper.Synchronizer
{
    // TODO: turn this into a timer based background task instead
    // TODO: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio#timed-background-tasks
    public class SleeperRefreshWorker : BackgroundService
    {
        private const string SleeperLastUpdatedKey = "SleeperLastUpdated";
        
        private readonly ILogger<SleeperRefreshWorker> _logger;
        private readonly ISleeperClient _sleeperClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private readonly ActivitySource _activitySource;
        private readonly TimeSpan _sleeperUpdatePeriod = TimeSpan.FromDays(1);

        public SleeperRefreshWorker(ILogger<SleeperRefreshWorker> logger,
            ISleeperClient sleeperClient,
            IServiceProvider serviceProvider,
            IConnectionMultiplexer redis,
            ActivitySource activitySource)
        {
            _logger = logger;
            _sleeperClient = sleeperClient;
            _serviceProvider = serviceProvider;
            _redis = redis;
            _activitySource = activitySource;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await MigrateDatabaseAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var nextUpdate = await UpdateSleeperPlayersIfNeededAsync(stoppingToken);
                var delay = nextUpdate - DateTime.Now;
                _logger.LogInformation("Will sleep for {}", delay);
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task<DateTime> UpdateSleeperPlayersIfNeededAsync(CancellationToken cancellationToken)
        {
            using var activity = _activitySource.StartActivity();
            await using var scope = _serviceProvider.CreateAsyncScope();
            
            var redisDatabase = _redis.GetDatabase();
            string rawSleeperLastUpdated = await redisDatabase.StringGetAsync(SleeperLastUpdatedKey);
            var sleeperLastUpdated = string.IsNullOrEmpty(rawSleeperLastUpdated)
                ? DateTime.MinValue
                : DateTime.Parse(rawSleeperLastUpdated);
            var nextUpdate = sleeperLastUpdated.Add(_sleeperUpdatePeriod);
                
            _logger.LogInformation("Sleeper last updated at [{}]. Sleeper next update at [{}]", sleeperLastUpdated, nextUpdate);

            if (nextUpdate <= DateTime.Now)
            {
                await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var players = await _sleeperClient.GetPlayersAsync();
                var existingPlayers = await databaseContext
                    .SleeperPlayers
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var player in players.Values)
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

                await databaseContext.SaveChangesAsync(cancellationToken);
                var lastUpdated = DateTime.Now;
                nextUpdate = lastUpdated.Add(_sleeperUpdatePeriod);
            
                await redisDatabase.StringSetAsync(SleeperLastUpdatedKey, lastUpdated.ToString("O"));
                _logger.LogInformation("Added/updated all players in database. Next update will be at [{}]", nextUpdate);
            }

            return nextUpdate;
        }
        
        private async Task MigrateDatabaseAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            using var migrationActivity = _activitySource.StartActivity();
            await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            await databaseContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
