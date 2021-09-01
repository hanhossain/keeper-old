using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Keeper.Core.Services;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var playerService = new PlayerService();
            var players = await playerService.GetPlayersAsync();
            Console.WriteLine($"{players.Count} players");

            foreach (var player in players.Where(x => x.Position == "DEF"))
            {
                Console.WriteLine(JsonSerializer.Serialize(player));
            }
        }
    }
}
