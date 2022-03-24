using System.Collections.Generic;

namespace Keeper.Core.Database.Models
{
    public class NflPlayer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string Team { get; set; }

        #region Navigation Properties

        public List<NflPlayerStatistics> PlayerStatistics { get; set; }

        public List<NflKickingStatistics> NflKickingStatistics { get; set; }
        
        #endregion
    }
}
