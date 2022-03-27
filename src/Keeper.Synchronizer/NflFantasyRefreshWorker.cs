using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using Keeper.Synchronizer.Nfl;
using Keeper.Synchronizer.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Keeper.Synchronizer;

public class NflFantasyRefreshWorker : BackgroundService
{
    private const string NflFantasyLastUpdatedKey = "NflFantasyLastUpdated";
    
    private readonly ILogger<NflFantasyRefreshWorker> _logger;
    private readonly IFantasyClient _fantasyClient;
    private readonly ActivitySource _activitySource;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRedisClient _redisClient;
    private readonly TimeSpan _updatePeriod = TimeSpan.FromDays(1);

    public NflFantasyRefreshWorker(
        ILogger<NflFantasyRefreshWorker> logger,
        IFantasyClient fantasyClient,
        ActivitySource activitySource,
        IServiceProvider serviceProvider,
        IRedisClient redisClient)
    {
        _logger = logger;
        _fantasyClient = fantasyClient;
        _activitySource = activitySource;
        _serviceProvider = serviceProvider;
        _redisClient = redisClient;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextUpdate = await UpdateNflFantasyAsync(stoppingToken);
            var delay = nextUpdate - DateTime.Now;
            _logger.LogInformation("Sleeping for {}", delay);
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task<DateTime> UpdateNflFantasyAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceProvider.CreateAsyncScope();

        var lastUpdated = await _redisClient.GetDateAsync(NflFantasyLastUpdatedKey) ?? DateTime.MinValue;
        var nextUpdate = lastUpdated.Add(_updatePeriod);

        if (nextUpdate <= DateTime.Now)
        {
            await using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var tasks = Enumerable.Range(1, 17).Select(x => _fantasyClient.GetAsync(2021, x));
            var results = await Task.WhenAll(tasks);

            var existingPlayers = await databaseContext
                .NflPlayers
                .ToDictionaryAsync(x => x.Id, cancellationToken);
            var existingPlayerStatistics = await databaseContext
                .NflPlayerStatistics
                .ToDictionaryAsync(x => (x.PlayerId, x.Season, x.Week), cancellationToken);
            var existingKickingStatistics = await databaseContext
                .NflKickingStatistics
                .ToDictionaryAsync(x => (x.PlayerId, x.Season, x.Week), cancellationToken);
            var existingDefensiveStatistics = await databaseContext
                .NflDefensiveStatistics
                .ToDictionaryAsync(x => (x.PlayerId, x.Season, x.Week), cancellationToken);
            var existingOffensiveStatistics = await databaseContext
                .NflOffensiveStatistics
                .ToDictionaryAsync(x => (x.PlayerId, x.Season, x.Week), cancellationToken);

            foreach (var player in results.SelectMany(x => x))
            {
                if (!existingPlayers.TryGetValue(player.Id, out var dbPlayer))
                {
                    dbPlayer = new NflPlayer()
                    {
                        Id = player.Id
                    };

                    existingPlayers[player.Id] = dbPlayer;
                    databaseContext.NflPlayers.Add(dbPlayer);
                }

                dbPlayer.Name = player.Name;
                dbPlayer.Position = player.Position;
                dbPlayer.Team = player.Team?.Name;
                
                // override Team=LA to LAR
                if (dbPlayer.Team == "LA")
                {
                    dbPlayer.Team = "LAR";
                }

                var compoundKey = (player.Id, player.Season, player.Week);

                if (!existingPlayerStatistics.TryGetValue(compoundKey, out var dbPlayerStatistics))
                {
                    dbPlayerStatistics = new NflPlayerStatistics()
                    {
                        PlayerId = player.Id,
                        Season = player.Season,
                        Week = player.Week
                    };

                    existingPlayerStatistics[compoundKey] = dbPlayerStatistics;
                    databaseContext.NflPlayerStatistics.Add(dbPlayerStatistics);
                }

                dbPlayerStatistics.FantasyPoints = player.Statistics.FantasyPoints;

                var kickingStats = player.Statistics.Kicking;
                if (kickingStats != null)
                {
                    if (!existingKickingStatistics.TryGetValue(compoundKey, out var dbKickingStatistics))
                    {
                        dbKickingStatistics = new NflKickingStatistics()
                        {
                            PlayerId = player.Id,
                            Season = player.Season,
                            Week = player.Week
                        };

                        existingKickingStatistics[compoundKey] = dbKickingStatistics;
                        databaseContext.NflKickingStatistics.Add(dbKickingStatistics);
                    }

                    dbKickingStatistics.PatMade = kickingStats.Pat.Made;
                    dbKickingStatistics.FieldGoal0To19Yards = kickingStats.FieldGoal.Yards0To19;
                    dbKickingStatistics.FieldGoal20To29Yards = kickingStats.FieldGoal.Yards20To29;
                    dbKickingStatistics.FieldGoal30To39Yards = kickingStats.FieldGoal.Yards30To39;
                    dbKickingStatistics.FieldGoal40To49Yards = kickingStats.FieldGoal.Yards40To49;
                    dbKickingStatistics.FieldGoal50PlusYards = kickingStats.FieldGoal.Yards50Plus;
                }

                var defensiveStats = player.Statistics.Defensive;
                if (defensiveStats != null)
                {
                    if (!existingDefensiveStatistics.TryGetValue(compoundKey, out var dbDefensiveStatistics))
                    {
                        dbDefensiveStatistics = new NflDefensiveStatistics()
                        {
                            PlayerId = player.Id,
                            Season = player.Season,
                            Week = player.Week
                        };

                        existingDefensiveStatistics[compoundKey] = dbDefensiveStatistics;
                        databaseContext.NflDefensiveStatistics.Add(dbDefensiveStatistics);
                    }

                    dbDefensiveStatistics.Sacks = defensiveStats.Tackling.Sacks;
                    dbDefensiveStatistics.Interceptions = defensiveStats.Turnover.Interceptions;
                    dbDefensiveStatistics.FumblesRecovered = defensiveStats.Turnover.FumblesRecovered;
                    dbDefensiveStatistics.Safeties = defensiveStats.Score.Safeties;
                    dbDefensiveStatistics.Touchdowns = defensiveStats.Score.Touchdowns;
                    dbDefensiveStatistics.Def2PtRet = defensiveStats.Score.Def2PtRet;
                    dbDefensiveStatistics.RetTouchdowns = defensiveStats.Returning.Touchdowns;
                    dbDefensiveStatistics.PointsAllowed = defensiveStats.Points.PointsAllowed;
                }

                var offensiveStats = player.Statistics.Offensive;
                if (offensiveStats != null)
                {
                    if (!existingOffensiveStatistics.TryGetValue(compoundKey, out var dbNflOffensiveStatistics))
                    {
                        dbNflOffensiveStatistics = new NflOffensiveStatistics()
                        {
                            PlayerId = player.Id,
                            Season = player.Season,
                            Week = player.Week
                        };

                        existingOffensiveStatistics[compoundKey] = dbNflOffensiveStatistics;
                        databaseContext.NflOffensiveStatistics.Add(dbNflOffensiveStatistics);
                    }

                    dbNflOffensiveStatistics.PassingYards = offensiveStats.Passing.Yards;
                    dbNflOffensiveStatistics.PassingTouchdowns = offensiveStats.Passing.Touchdowns;
                    dbNflOffensiveStatistics.PassingInterceptions = offensiveStats.Passing.Interceptions;
                    dbNflOffensiveStatistics.RushingYards = offensiveStats.Rushing.Yards;
                    dbNflOffensiveStatistics.RushingTouchdowns = offensiveStats.Rushing.Touchdowns;
                    dbNflOffensiveStatistics.ReceivingReceptions = offensiveStats.Receiving.Receptions;
                    dbNflOffensiveStatistics.ReceivingYards = offensiveStats.Receiving.Yards;
                    dbNflOffensiveStatistics.ReceivingTouchdowns = offensiveStats.Receiving.Touchdowns;
                    dbNflOffensiveStatistics.ReturningTouchdowns = offensiveStats.Returning.Touchdowns;
                    dbNflOffensiveStatistics.TwoPointConversions = offensiveStats.Miscellaneous.TwoPointConversions;
                    dbNflOffensiveStatistics.FumbleTouchdowns = offensiveStats.Fumble.Touchdowns;
                    dbNflOffensiveStatistics.FumblesLost = offensiveStats.Fumble.Lost;
                }
            }

            await databaseContext.SaveChangesAsync(cancellationToken);
            lastUpdated = DateTime.Now;
            nextUpdate = lastUpdated.Add(_updatePeriod);

            await _redisClient.SetAsync(NflFantasyLastUpdatedKey, lastUpdated);
            _logger.LogInformation("Updated NFL Fantasy data in database. Next update will be at [{}]", nextUpdate);
        }

        return nextUpdate;
    }
}
