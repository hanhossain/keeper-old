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
                });
            
            await Task.WhenAll(databaseTasks);
        }
    }
}
