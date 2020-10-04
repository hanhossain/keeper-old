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

            var players = await fantasyClient.GetAsync(Position.Defense, 2020, 3, 0);

            foreach (var player in players.Values)
            {
                if (player.Statistics is DefensiveStatistics statistics)
                {
                    Console.WriteLine($"{player.Name} - {player.Team?.Name} - {statistics.FantasyPoints} - {statistics.Points.PointsAllowed}");
                }
            }

            Console.WriteLine(players.TotalCount);
        }
    }
}