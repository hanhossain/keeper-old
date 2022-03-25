using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Keeper.Synchronizer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISleeperClient _sleeperClient;
        private readonly DatabaseContext _databaseContext;

        public Worker(ILogger<Worker> logger, ISleeperClient sleeperClient, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sleeperClient = sleeperClient;
            _databaseContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _databaseContext.Database.MigrateAsync(stoppingToken);
            
            if (await _databaseContext.SleeperPlayers.AnyAsync(stoppingToken))
            {
                _logger.LogInformation("Players exist in database");
                return;
            }
            
            var players = await _sleeperClient.GetPlayersAsync();
            _logger.LogInformation("Got all players from sleeper");
            var dbPlayers = players
                .Values
                .Select(x => new SleeperPlayer()
                {
                    Active = x.Active,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Id = x.PlayerId,
                    Position = x.Position,
                    Status = x.Status,
                    Team = x.Team
                })
                .ToList();
            _databaseContext.SleeperPlayers.AddRange(dbPlayers);
            await _databaseContext.SaveChangesAsync(stoppingToken);
            
            _logger.LogInformation("Saved all players to database");
        }
    }
}
