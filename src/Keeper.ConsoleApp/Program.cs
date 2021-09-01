using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Keeper.Core.Nfl;
using Keeper.Core.Services;
using Keeper.Core.Sleeper;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using var httpClient = new HttpClient();
            var sleeperClient = new SleeperClient(httpClient);
            var fantasyClient = new FantasyClient(httpClient);
            var playerService = new PlayerService(sleeperClient, fantasyClient);
            var players = await playerService.GetPlayersAsync();
            Console.WriteLine($"{players.Count} players");

            foreach (var player in players.Where(x => x.Position == "DEF"))
            {
                Console.WriteLine(JsonSerializer.Serialize(player));
            }
        }
    }
}
