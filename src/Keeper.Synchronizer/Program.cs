using Keeper.Core.Database;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Keeper.Synchronizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.AddHttpClient<ISleeperClient, SleeperClient>();
                    
                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlServer(config.GetConnectionString("DatabaseContext")));
                    services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(
                        _ => ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")));

                    services.AddHostedService<SleeperRefreshWorker>();
                });
    }
}
