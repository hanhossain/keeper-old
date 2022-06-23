using AngleSharp.Dom;

namespace Keeper.Synchronizer.Nfl.Models.Statistics
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