using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Keeper.Synchronizer.Redis;

public class RedisClient : IRedisClient
{
    private readonly IDatabase _redis;

    public RedisClient(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task<DateTime?> GetDateAsync(string key)
    {
        string rawDate = await _redis.StringGetAsync(key);
        return DateTime.TryParse(rawDate, out var result) ? result : null;
    }

    public async Task SetAsync(string key, DateTime dateTime)
    {
        var rawDate = dateTime.ToString("O");
        await _redis.StringSetAsync(key, rawDate);
    }
}
