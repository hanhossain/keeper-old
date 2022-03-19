using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Core.Database.Models;

namespace Keeper.Core.Database
{
    public interface ISleeperCache
    {
        Task RefreshPlayersAsync();

        Task<List<SleeperPlayer>> GetPlayersAsync();
    }
}
