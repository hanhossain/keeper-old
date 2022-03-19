namespace Keeper.Core.Database.Models
{
    public class NflOffensiveStatistics
    {
        public int PlayerId { get; set; }

        public int Season { get; set; }

        public int Week { get; set; }

        public int? PassingYards { get; set; }

        public int? PassingTouchdowns { get; set; }

        public int? PassingInterceptions { get; set; }

        public int? RushingYards { get; set; }

        public int? RushingTouchdowns { get; set; }

        public int? ReceivingReceptions { get; set; }

        public int? ReceivingYards { get; set; }

        public int? ReceivingTouchdowns { get; set; }

        public int? ReturningTouchdowns { get; set; }

        public int? TwoPointConversions { get; set; }

        public int? FumbleTouchdowns { get; set; }

        public int? FumblesLost { get; set; }

        #region Navigation Properteis

        public NflPlayer Player { get; set; }

        #endregion
    }
}
