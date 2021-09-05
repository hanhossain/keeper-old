using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
{
    public class OffensiveStatistics
    {
        public OffensiveStatistics(IElement row)
        {
            Passing = new PassingStatistics(row);
            Rushing = new RushingStatistics(row);
            Receiving = new ReceivingStatistics(row);
            Returning = new ReturningStatistics(row, true);
            Miscellaneous = new MiscellaneousStatistics(row);
            Fumble = new FumbleStatistics(row);
        }

        public PassingStatistics Passing { get; }

        public RushingStatistics Rushing { get; }
        
        public ReceivingStatistics Receiving { get; }
        
        public ReturningStatistics Returning { get; }

        public MiscellaneousStatistics Miscellaneous { get; }

        public FumbleStatistics Fumble { get; }
    }
}