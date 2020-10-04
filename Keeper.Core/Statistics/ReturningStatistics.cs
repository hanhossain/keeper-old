using AngleSharp.Dom;

namespace Keeper.Core.Statistics
{
    public class ReturningStatistics
    {
        public ReturningStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_28").TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
        }
        
        public int? Touchdowns { get; }
    }
}