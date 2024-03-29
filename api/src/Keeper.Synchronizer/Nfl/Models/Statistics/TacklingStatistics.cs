using AngleSharp.Dom;

namespace Keeper.Synchronizer.Nfl.Models.Statistics
{
    public class TacklingStatistics
    {
        public TacklingStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_45").TextContent, out var sacks))
            {
                Sacks = sacks;
            }
        }

        public int? Sacks { get; }
    }
}