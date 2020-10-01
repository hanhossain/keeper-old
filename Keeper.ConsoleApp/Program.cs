using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Text;
using Keeper.Core;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var fantasyClient = new FantasyClient(httpClient);

            // Console.WriteLine(await fantasyClient.TotalPlayersAsync(Position.Offense, 2020, 1, 0));
            //
            //
            var quarterbacks = await fantasyClient.GetAsync(Position.Quarterback, 2020, 1, 0);

            foreach (var qb in quarterbacks.Values)
            {
                Console.WriteLine($"{qb.Id} - {qb.Name} - {qb.Position} - {qb.Team?.Name} - {qb.Team?.Opponent} - {qb.Team?.Location} - {qb.Points}");
            }

            Console.WriteLine(quarterbacks.TotalCount);
            // await using var file = File.OpenRead("/Users/hanhossain/Developer/Keeper/Keeper.ConsoleApp/quarterbacks.html");
            //
            // var config = Configuration.Default;
            // var context = BrowsingContext.New(config);
            // var document = await context.OpenAsync(x => x.Content(file));
            //
            // var tableBody = document.QuerySelector("table tbody");
            //
            // foreach (var row in tableBody.QuerySelectorAll("tr"))
            // {
            //     Console.WriteLine(row.QuerySelector(".playerName").TextContent);
            // }
        }
    }
}