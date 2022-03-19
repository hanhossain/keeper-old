using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Models.Statistics
{
    public class PassingStatistics
    {
        public PassingStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_5").TextContent, out var passingYards))
            {
                Yards = passingYards;
            }

            if (int.TryParse(row.QuerySelector(".stat_6").TextContent, out var passingTouchdowns))
            {
                Touchdowns = passingTouchdowns;
            }

            if (int.TryParse(row.QuerySelector(".stat_7").TextContent, out var passingInterceptions))
            {
                Interceptions = passingInterceptions;
            }
        }
        
        public int? Yards { get; }
        
        public int? Touchdowns { get; }
        
        public int? Interceptions { get; }
    }
}