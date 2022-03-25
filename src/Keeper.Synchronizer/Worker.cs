using System;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Keeper.Synchronizer
{
    // TODO: turn this into a timer based background task instead
    // TODO: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio#timed-background-tasks
    public class Worker : BackgroundService
    {
        private const string SleeperLastUpdatedKey = "SleeperLastUpdated";
        
        private readonly ILogger<Worker> _logger;
        private readonly ISleeperClient _sleeperClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _sleeperUpdatePeriod = TimeSpan.FromMinutes(5);

        public Worker(ILogger<Worker> logger, ISleeperClient sleeperClient, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _sleeperClient = sleeperClient;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var redis = await ConnectionMultiplexer.ConnectAsync(_configuration.GetConnectionString("Redis"));
            var redisDatabase = redis.GetDatabase();

            await using var scope = _serviceProvider.CreateAsyncScope();
            await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            await databaseContext.Database.MigrateAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                string rawSleeperLastUpdated = await redisDatabase.StringGetAsync(SleeperLastUpdatedKey);
                var sleeperLastUpdated = string.IsNullOrEmpty(rawSleeperLastUpdated)
                    ? DateTime.MinValue
                    : DateTime.Parse(rawSleeperLastUpdated);
                var nextUpdate = sleeperLastUpdated.Add(_sleeperUpdatePeriod);
                
                _logger.LogInformation("Sleeper last updated at [{}]. Sleeper next update at [{}]", sleeperLastUpdated, nextUpdate);

                if (nextUpdate <= DateTime.Now)
                {
                    var players = await _sleeperClient.GetPlayersAsync();
                    var existingPlayers =
                        await databaseContext.SleeperPlayers.ToDictionaryAsync(x => x.Id, stoppingToken);

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
                        dbPlayer.FullName = $"{player.FirstName} ${player.LastName}";
                    }

                    await databaseContext.SaveChangesAsync(stoppingToken);
                    sleeperLastUpdated = DateTime.Now;
                    nextUpdate = sleeperLastUpdated.Add(_sleeperUpdatePeriod);
                    await redisDatabase.StringSetAsync(SleeperLastUpdatedKey, sleeperLastUpdated.ToString("O"));
                    _logger.LogInformation("Added/updated all players in database. Next update will be at [{}]", nextUpdate);
                }

                var delay = nextUpdate - DateTime.Now;
                _logger.LogInformation("Will sleep for {}", delay);
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
