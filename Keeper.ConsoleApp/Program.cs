using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Text;
using Keeper.Core;
using Keeper.Core.Statistics;

namespace Keeper.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var fantasyClient = new FantasyClient(httpClient);

            var quarterbacks = await fantasyClient.GetAsync(Position.Quarterback, 2020, 3, 0);

            foreach (var qb in quarterbacks.Values)
            {
                if (qb.Statistics is OffensiveStatistics offensiveStatistics)
                {
                    Console.WriteLine($"{qb.Name} - {qb.Team?.Name} - {offensiveStatistics.FantasyPoints} - {offensiveStatistics.Passing.Yards}");
                }
            }

            Console.WriteLine(quarterbacks.TotalCount);
        }
    }
}