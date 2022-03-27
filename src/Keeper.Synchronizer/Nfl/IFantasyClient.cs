using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Synchronizer.Nfl.Models;

namespace Keeper.Synchronizer.Nfl
{
    public interface IFantasyClient
    {
        Task<NflPageResult> GetAsync(int season, int week, int offset, NflPosition position);

        Task<List<NflPageResult>> GetAsync(int season, int week, NflPosition position);

        Task<List<NflPlayer>> GetAsync(int season, int week);
    }
}
