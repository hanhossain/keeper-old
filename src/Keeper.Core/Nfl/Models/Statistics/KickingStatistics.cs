using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Models.Statistics
{
    public class KickingStatistics
    {
        public KickingStatistics(IElement row)
        {
            Pat = new PatStatistics(row);
            FieldGoal = new FieldGoalStatistics(row);
        }
        
        public PatStatistics Pat { get; }

        public FieldGoalStatistics FieldGoal { get; }
    }
}