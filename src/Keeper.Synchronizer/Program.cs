using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Keeper.Synchronizer.Sleeper;

namespace Keeper.Synchronizer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            var sleeperClient = new SleeperClient(httpClient);

            var players = await sleeperClient.GetPlayersAsync();
            var mahomes = players.Values.Where(x => x.LastName == "Mahomes").First();
            Console.WriteLine($"{mahomes.FirstName} {mahomes.LastName} {mahomes.Team}");

            var hill = players.Values.Where(x => x.LastName == "Hill" && x.FirstName == "Tyreek").First();
            Console.WriteLine($"{hill.FirstName} {hill.LastName} {hill.Team}");
        }
    }
}

