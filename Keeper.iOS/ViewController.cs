using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Keeper.Core;
using UIKit;

namespace Keeper.iOS
{
    public class ViewController : UIViewController
    {
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;

            Console.WriteLine("Hello world from view controller");

            await RunAsync();
        }

        public async Task RunAsync()
        {
            using var httpClient = new HttpClient();
            var fantasyClient = new FantasyClient(httpClient);

            var stopwatch = Stopwatch.StartNew();
            await SerialAsync(fantasyClient);
            stopwatch.Stop();

            Console.WriteLine($"Serial: {stopwatch.ElapsedMilliseconds / 1000.0} seconds");

            var stopwatch1 = Stopwatch.StartNew();
            await ParallelAsync(fantasyClient);
            stopwatch1.Stop();

            Console.WriteLine($"Parallel: {stopwatch1.ElapsedMilliseconds / 1000.0} seconds");
        }

        private async Task SerialAsync(FantasyClient client)
        {
            for (int i = 1; i <= 3; i++)
            {
                await client.GetAsync(2020, i);
            }
        }

        private async Task ParallelAsync(FantasyClient client)
        {
            var tasks = new List<Task>();

            for (int i = 1; i <= 3; i++)
            {
                tasks.Add(client.GetAsync(2020, i));
            }

            await Task.WhenAll(tasks);
        }
    }
}
