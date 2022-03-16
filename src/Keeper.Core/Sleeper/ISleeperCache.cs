using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Core.Database;

namespace Keeper.Core.Sleeper
{
    public interface ISleeperCache
    {
        Task RefreshPlayersAsync();

        Task<List<SleeperPlayer>> GetPlayersAsync();
    }
}
