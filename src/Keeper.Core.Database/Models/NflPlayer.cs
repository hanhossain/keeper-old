using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keeper.Core.Database.Models
{
    public class NflPlayer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string Team { get; set; }

        #region Navigation Properties

        public ICollection<NflPlayerStatistics> PlayerStatistics { get; set; }

        public ICollection<NflKickingStatistics> NflKickingStatistics { get; set; }

        public ICollection<NflDefensiveStatistics> NflDefensiveStatistics { get; set; }

        public ICollection<NflOffensiveStatistics> NflOffensiveStatistics { get; set; }

        public SleeperPlayer SleeperPlayer { get; set; }

        #endregion
    }
}
