using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
{
    public class PatStatistics
    {
        public PatStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_33").TextContent, out var made))
            {
                Made = made;
            }
        }

        public int? Made { get; }
    }
}