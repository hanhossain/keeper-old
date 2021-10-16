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

            await using var dbContext = new DatabaseContext();

            foreach (var seasonResults in results)
            {
                foreach (var weekResults in seasonResults)
                {
                    foreach (var playerResult in weekResults.Values)
                    {
                        if (!await dbContext.NflPlayers.AnyAsync(x => x.Id == playerResult.Id))
                        {
                            var player = new NflPlayer()
                            {
                                Id = playerResult.Id,
                                Name = playerResult.Name,
                                Position = playerResult.Position,
                                Team = playerResult.Team?.Name
                            };

                            dbContext.NflPlayers.Add(player);
                            await dbContext.SaveChangesAsync();
                        }

                        if (!await dbContext.NflPlayerStatistics.AnyAsync(x =>
                            x.PlayerId == playerResult.Id && x.Season == weekResults.Season &&
                            x.Week == weekResults.Week))
                        {
                            var statistics = new NflPlayerStatistics()
                            {
                                PlayerId = playerResult.Id,
                                Season = weekResults.Season,
                                Week = weekResults.Week,
                                FantasyPoints = playerResult.Statistics?.FantasyPoints ?? 0
                            };

                            dbContext.NflPlayerStatistics.Add(statistics);
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}
