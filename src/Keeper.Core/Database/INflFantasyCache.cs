using System.Threading.Tasks;

namespace Keeper.Core.Database
{
    public interface INflFantasyCache
    {
        Task RefreshStatisticsAsync();
    }
}
