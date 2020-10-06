using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Keeper.Core;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var fantasyClient = new FantasyClient(httpClient);

            var stopwatch = Stopwatch.StartNew();
            var all = await fantasyClient.GetAsync(2020, 4);
            stopwatch.Stop();

            var players = all.Where(x => x.Team?.Name == "KC").ToList();
            foreach (var player in players)
            {
                Console.WriteLine($"{player.Name} - {player.Statistics.FantasyPoints}");
            }

            Console.WriteLine($"Total points: {players.Sum(x => x.Statistics.FantasyPoints)}");
            Console.WriteLine($"Retrieved {all.Count} in {stopwatch.ElapsedMilliseconds / 1000.0} seconds");
        }
    }
}