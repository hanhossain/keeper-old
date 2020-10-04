using AngleSharp.Dom;

namespace Keeper.Core.Statistics
{
    public class PointsStatistics
    {
        public PointsStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_54").TextContent, out var pointsAllowed))
            {
                PointsAllowed = pointsAllowed;
            }
        }
        
        public int? PointsAllowed { get; }
    }
}