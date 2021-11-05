using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Keeper.Core.Database;
using Keeper.Core.Nfl;
using Microsoft.EntityFrameworkCore;
using NflPlayer = Keeper.Core.Database.NflPlayer;

namespace Keeper.Core
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var command = new Command(args);
            
            await MigrateDatabaseAsync();
            
            if (command.UpdateNfl)
            {
                await UpdateNflAsync();
            }
        }

        private static async Task UpdateNflAsync()
        {
            using var httpClient = new HttpClient();
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
                    });

                await Task.WhenAll(databaseTasks);
        }

        private static async Task MigrateDatabaseAsync()
        {
            using var context = new DatabaseContext();
            await context.Database.MigrateAsync();
        }
    }
}
