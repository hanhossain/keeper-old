using AngleSharp.Dom;

namespace Keeper.Core.Statistics
{
    public class KickingStatistics : PlayerStatistics
    {
        public KickingStatistics(IElement row)
            : base(row)
        {
            Pat = new PatStatistics(row);
            FieldGoal = new FieldGoalStatistics(row);
        }
        
        public PatStatistics Pat { get; }

        public FieldGoalStatistics FieldGoal { get; }
    }
}