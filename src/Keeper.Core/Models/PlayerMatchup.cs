using Keeper.Core.Nfl.Statistics;

namespace Keeper.Core.Models
{
    public class PlayerMatchup
    {
        public Player Player { get; set; }

        public PlayerStatistics Statistics { get; set; }
    }
}
