using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
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