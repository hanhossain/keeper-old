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

            Console.WriteLine(await fantasyClient.TotalPlayersAsync(Position.Offense, 2020, 1, 0));
        }
    }
}