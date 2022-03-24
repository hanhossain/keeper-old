using AngleSharp.Dom;

namespace Keeper.Synchronizer.Nfl.Models.Statistics
{
    public class ReceivingStatistics
    {
        public ReceivingStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_20").TextContent, out var receptions))
            {
                Receptions = receptions;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_21").TextContent, out var yards))
            {
                Yards = yards;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_22").TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
        }
        
        public int? Receptions { get; }
        
        public int? Yards { get; }
        
        public int? Touchdowns { get; }
    }
}