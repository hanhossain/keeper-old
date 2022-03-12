using AngleSharp.Dom;

namespace Keeper.ConsoleApp.Nfl.Statistics
{
    public class ReturningStatistics
    {
        public ReturningStatistics(IElement row, bool isOffense)
        {
            var selector = isOffense ? ".stat_28" : ".stat_53";
            if (int.TryParse(row.QuerySelector(selector).TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
        }
        
        public int? Touchdowns { get; }
    }
}