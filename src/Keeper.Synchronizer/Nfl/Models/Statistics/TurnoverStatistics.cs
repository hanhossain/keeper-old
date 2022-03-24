using AngleSharp.Dom;

namespace Keeper.Synchronizer.Nfl.Models.Statistics
{
    public class TurnoverStatistics
    {
        public TurnoverStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_46").TextContent, out var interceptions))
            {
                Interceptions = interceptions;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_47").TextContent, out var fumblesRecovered))
            {
                FumblesRecovered = fumblesRecovered;
            }
        }
        
        public int? Interceptions { get; }

        public int? FumblesRecovered { get; }
    }
}