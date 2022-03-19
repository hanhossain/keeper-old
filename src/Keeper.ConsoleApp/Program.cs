using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Nfl;
using Keeper.Core.Sleeper;
using Microsoft.EntityFrameworkCore;
using DatabaseContext = Keeper.ConsoleApp.Database.DatabaseContext;
using NflPlayer = Keeper.Core.Database.NflPlayer;
using SleeperPlayer = Keeper.ConsoleApp.Database.SleeperPlayer;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var command = new Command(args);
            
            await MigrateDatabaseAsync();

            using var httpClient = new HttpClient();
            var tasks = new List<Task>();
            
            if (command.UpdateNfl)
            {
                tasks.Add(UpdateNflAsync(httpClient));
            }

            if (command.UpdateSleeper)
            {
                tasks.Add(UpdateSleeperAsync(httpClient));
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Error.WriteLine("No arguments were passed in");
                Console.ResetColor();
            }
        }

        private static async Task UpdateSleeperAsync(HttpClient httpClient)
        {
            Console.WriteLine("Updating Sleeper data...");
            var stopwatch = Stopwatch.StartNew();

            var sleeperClient = new SleeperClient(httpClient);
            var players = await sleeperClient.GetPlayersAsync();

            var databaseTasks = players
                .Values
                .Select(async x =>
                {
                    using var dbContext = new DatabaseContext();

                    var player = await dbContext.SleeperPlayers.FindAsync(x.PlayerId);
                    bool isUpdate = true;

                    if (player == null)
                    {
                        isUpdate = false;
                        player = new SleeperPlayer()
                        {
                            Id = x.PlayerId
                        };
                    }

                    player.Active = x.Active;
                    player.FirstName = x.FirstName;
                    player.FullName = x.FullName;
                    player.LastName = x.LastName;
                    player.Position = x.Position;
                    player.Status = x.Status;
                    player.Team = x.Team;

                    if (!isUpdate)
                    {
                        dbContext.SleeperPlayers.Add(player);
                    }

                    await dbContext.SaveChangesAsync();
                });

            await Task.WhenAll(databaseTasks);

            stopwatch.Stop();
            Console.WriteLine($"Updating Sleeper data... Done! Completed in {stopwatch.ElapsedMilliseconds} ms.");
        }

        private static async Task UpdateNflAsync(HttpClient httpClient)
        {
            Console.WriteLine("Updating NFL data...");
            var stopwatch = Stopwatch.StartNew();

            var fantasyClient = new FantasyClient(httpClient);

            var tasks = new[]
            {
                fantasyClient.GetAsync(2021, NflPosition.Quarterback),
                fantasyClient.GetAsync(2021, NflPosition.RunningBack),
                fantasyClient.GetAsync(2021, NflPosition.WideReceiver),
                fantasyClient.GetAsync(2021, NflPosition.TightEnd),
                fantasyClient.GetAsync(2021, NflPosition.Kicker),
                fantasyClient.GetAsync(2021, NflPosition.Defense)
            };

            var results = await Task.WhenAll(tasks);

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

        private static async Task MigrateDatabaseAsync()
        {
            await using var context = new DatabaseContext();
            var migrations = await context.Database.GetPendingMigrationsAsync();

            if (migrations.Any())
            {
                Console.WriteLine("Migrating database...");
                var stopwatch = Stopwatch.StartNew();

                await context.Database.MigrateAsync();
                stopwatch.Stop();
                Console.WriteLine($"Migrating database... Done! Completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }
    }
}
