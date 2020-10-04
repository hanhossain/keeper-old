using System;
using System.Diagnostics;
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
            var all = await fantasyClient.GetAsync(2019, 1);
            stopwatch.Stop();
            
            Console.WriteLine($"Retrieved {all.Count} in {stopwatch.ElapsedMilliseconds / 1000.0} seconds");
        }
    }
}