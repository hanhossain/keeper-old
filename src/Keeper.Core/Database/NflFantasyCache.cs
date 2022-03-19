using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Keeper.Core.Database.Models;
using Keeper.Core.Nfl;
using Microsoft.EntityFrameworkCore;
using NflPosition = Keeper.Core.Nfl.Models.NflPosition;

namespace Keeper.Core.Database
{
    public class NflFantasyCache : INflFantasyCache
    {
        // TODO: don't use a hardcoded season
        private const int Season = 2021;
        private readonly IFantasyClient _fantasyClient;

        public NflFantasyCache(IFantasyClient fantasyClient)
        {
            _fantasyClient = fantasyClient;
        }

        public async Task RefreshStatisticsAsync()
        {
            Console.WriteLine("Getting NFL Data...");
            var stopwatch = Stopwatch.StartNew();

            var tasks = new[]
            {
                _fantasyClient.GetAsync(Season, NflPosition.Quarterback),
                _fantasyClient.GetAsync(Season, NflPosition.RunningBack),
                _fantasyClient.GetAsync(Season, NflPosition.WideReceiver),
                _fantasyClient.GetAsync(Season, NflPosition.TightEnd),
                _fantasyClient.GetAsync(Season, NflPosition.Kicker),
                _fantasyClient.GetAsync(Season, NflPosition.Defense)
            };

            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"Getting NFL Data... Done! Completed in {stopwatch.ElapsedMilliseconds} ms.");

            Console.WriteLine("Updating NFL Data...");
            stopwatch.Restart();
            var databaseTasks = results
                .SelectMany(x => x)
                .SelectMany(x => x.Values)
                .Select(async x =>
                {
                    await using var dbContext = new DatabaseContext();

                    if (!await dbContext.NflPlayers.AnyAsync(p => p.Id == x.Id))
                    {
                        var player = new NflPlayer()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Position = x.Position,
                            Team = x.Team?.Name
                        };

                        dbContext.NflPlayers.Add(player);
                        await dbContext.SaveChangesAsync();
                    }

                    if (!await dbContext.NflPlayerStatistics.AnyAsync(p =>
                            p.PlayerId == x.Id && p.Season == x.Season &&
                            p.Week == x.Week))
                    {
                        var statistics = new NflPlayerStatistics()
                        {
                            PlayerId = x.Id,
                            Season = x.Season,
                            Week = x.Week,
                            FantasyPoints = x.Statistics.FantasyPoints
                        };

                        dbContext.NflPlayerStatistics.Add(statistics);
                        await dbContext.SaveChangesAsync();
                    }

                    // TODO: need to support updating previously saved data
                    if (x.Statistics.Kicking != null && !await dbContext.NflKickingStatistics.AnyAsync(p =>
                            p.PlayerId == x.Id && p.Season == x.Season && p.Week == x.Week))
                    {
                        var kickingStatistics = new NflKickingStatistics()
                        {
                            PlayerId = x.Id,
                            Season = x.Season,
                            Week = x.Week,
                            PatMade = x.Statistics.Kicking.Pat.Made,
                            FieldGoal0To19Yards = x.Statistics.Kicking.FieldGoal.Yards0To19,
                            FieldGoal20To29Yards = x.Statistics.Kicking.FieldGoal.Yards20To29,
                            FieldGoal30To39Yards = x.Statistics.Kicking.FieldGoal.Yards30To39,
                            FieldGoal40To49Yards = x.Statistics.Kicking.FieldGoal.Yards40To49,
                            FieldGoal50PlusYards = x.Statistics.Kicking.FieldGoal.Yards50Plus
                        };

                        dbContext.NflKickingStatistics.Add(kickingStatistics);
                        await dbContext.SaveChangesAsync();
                    }

                    if (x.Statistics.Defensive != null && !await dbContext.NflDefensiveStatistics.AnyAsync(p =>
                            p.PlayerId == x.Id && p.Season == x.Season && p.Week == x.Week))
                    {
                        var defensiveStatistics = new NflDefensiveStatistics()
                        {
                            PlayerId = x.Id,
                            Season = x.Season,
                            Week = x.Week,
                            Sacks = x.Statistics.Defensive.Tackling.Sacks,
                            Interceptions = x.Statistics.Defensive.Turnover.Interceptions,
                            FumblesRecovered = x.Statistics.Defensive.Turnover.FumblesRecovered,
                            Safeties = x.Statistics.Defensive.Score.Safeties,
                            Touchdowns = x.Statistics.Defensive.Score.Touchdowns,
                            Def2PtRet = x.Statistics.Defensive.Score.Def2PtRet,
                            RetTouchdowns = x.Statistics.Defensive.Returning.Touchdowns,
                            PointsAllowed = x.Statistics.Defensive.Points.PointsAllowed
                        };

                        dbContext.NflDefensiveStatistics.Add(defensiveStatistics);
                        await dbContext.SaveChangesAsync();
                    }

                    if (x.Statistics.Offensive != null && !await dbContext.NflOffensiveStatistics.AnyAsync(p =>
                            p.PlayerId == x.Id && p.Season == x.Season && p.Week == x.Week))
                    {
                        var passing = x.Statistics.Offensive.Passing;
                        var rushing = x.Statistics.Offensive.Rushing;
                        var receiving = x.Statistics.Offensive.Receiving;
                        var fumbles = x.Statistics.Offensive.Fumble;
                        var returning = x.Statistics.Offensive.Returning;
                        var misc = x.Statistics.Offensive.Miscellaneous;

                        var offensiveStatistics = new NflOffensiveStatistics()
                        {
                            PlayerId = x.Id,
                            Season = x.Season,
                            Week = x.Week,
                            PassingInterceptions = passing.Interceptions,
                            PassingTouchdowns = passing.Touchdowns,
                            PassingYards = passing.Yards,
                            RushingTouchdowns = rushing.Touchdowns,
                            RushingYards = rushing.Yards,
                            ReceivingReceptions = receiving.Receptions,
                            ReceivingTouchdowns = receiving.Touchdowns,
                            ReceivingYards = receiving.Yards,
                            FumblesLost = fumbles.Lost,
                            FumbleTouchdowns = fumbles.Touchdowns,
                            ReturningTouchdowns = returning.Touchdowns,
                            TwoPointConversions = misc.TwoPointConversions
                        };

                        dbContext.NflOffensiveStatistics.Add(offensiveStatistics);
                        await dbContext.SaveChangesAsync();
                    }
                });

            await Task.WhenAll(databaseTasks);
            
            stopwatch.Stop();
            Console.WriteLine($"Updating NFL data... Done! Completed in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
