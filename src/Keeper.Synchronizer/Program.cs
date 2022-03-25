using Keeper.Core.Database;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Keeper.Synchronizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlServer(config.GetConnectionString("DatabaseContext")));
                    services.AddHttpClient<ISleeperClient, SleeperClient>();
                    services.AddHostedService<Worker>();
                });
    }
}
