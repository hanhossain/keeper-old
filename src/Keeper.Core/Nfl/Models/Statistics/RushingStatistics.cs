using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Models.Statistics
{
    public class RushingStatistics
    {
        public RushingStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_14").TextContent, out var yards))
            {
                Yards = yards;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_15").TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
        }
        
        public int? Yards { get; }
        
        public int? Touchdowns { get; }
    }
}