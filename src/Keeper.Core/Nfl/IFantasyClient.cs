using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.Core.Nfl
{
    public interface IFantasyClient
    {
        Task<NflPageResult> GetAsync(int season, int week, int offset, NflPosition? position = null);

        Task<List<NflPageResult>> GetAsync(int season, int week, NflPosition? position = null);

        Task<List<NflResult>> GetAsync(int season, NflPosition? position = null);
    }
}
