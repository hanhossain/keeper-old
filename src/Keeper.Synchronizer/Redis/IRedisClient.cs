using System;
using System.Threading.Tasks;

namespace Keeper.Synchronizer.Redis;

public interface IRedisClient
{
    Task<DateTime?> GetDateAsync(string key);

    Task SetAsync(string key, DateTime dateTime);
}
