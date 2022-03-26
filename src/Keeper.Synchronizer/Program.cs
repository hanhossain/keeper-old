using System.Diagnostics;
using Keeper.Core.Database;
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
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var serviceName = "keeper.synchronizer";
                    var serviceVersion = "1.0.0";
                    
                    var config = hostContext.Configuration;
                    
                    // Configure OpenTelemetry
                    services.AddOpenTelemetryTracing(builder =>
                        builder
                            .AddJaegerExporter(options => options.AgentHost = config.GetConnectionString("Jaeger"))
                            .AddSource(serviceName)
                            .SetResourceBuilder(
                                ResourceBuilder.CreateDefault()
                                    .AddService(serviceName, serviceVersion: serviceVersion))
                            .AddHttpClientInstrumentation()
                            .AddAspNetCoreInstrumentation()
                            .AddSqlClientInstrumentation()
                            .AddRedisInstrumentation());

                    services.AddSingleton<ActivitySource, ActivitySource>(_ => new ActivitySource(serviceName));
                    
                    services.AddHttpClient<ISleeperClient, SleeperClient>();
                    
                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlServer(config.GetConnectionString("DatabaseContext")));
                    services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(
                        _ => ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")));

                    services.AddHostedService<SleeperRefreshWorker>();
                });
    }
}
