using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.Core.Sleeper
{
    public interface ISleeperClient
    {
        Task<Dictionary<string, SleeperPlayer>> GetPlayersAsync();

        Task<SleeperNflState> GetNflStateAsync();
    }
}
