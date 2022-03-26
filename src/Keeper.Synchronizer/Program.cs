using System.Diagnostics;
using Keeper.Core.Database;
using Keeper.Synchronizer.Redis;
using Keeper.Synchronizer.Sleeper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace Keeper.Synchronizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            MigrateDatabase(host);
            
            host.Run();
        }

        private static void MigrateDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var activitySource = scope.ServiceProvider.GetRequiredService<ActivitySource>();
            using var activity = activitySource.StartActivity();

            using var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            databaseContext.Database.Migrate();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var serviceName = "keeper-synchronizer";
                    var config = hostContext.Configuration;

                    // Configure OpenTelemetry
                    services.AddOpenTelemetryTracing(builder =>
                        builder
                            .AddJaegerExporter(options => options.AgentHost = config.GetConnectionString("Jaeger"))
                            .AddSource(serviceName)
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                            .AddHttpClientInstrumentation()
                            .AddAspNetCoreInstrumentation()
                            .AddSqlClientInstrumentation()
                            .AddRedisInstrumentation());

                    services.AddSingleton<ActivitySource, ActivitySource>(_ => new ActivitySource(serviceName));

                    services.AddHttpClient<ISleeperClient, SleeperClient>();

                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlServer(config.GetConnectionString("DatabaseContext")));
                    services
                        .AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(
                            _ => ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")))
                        .AddSingleton<IRedisClient, RedisClient>();

                    services.AddHostedService<SleeperRefreshWorker>();
                });
    }
}
