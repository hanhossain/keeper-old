using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.Core.Database
{
    public interface ISleeperCache
    {
        Task RefreshPlayersAsync();

        Task<List<SleeperPlayer>> GetPlayersAsync();
    }
}
