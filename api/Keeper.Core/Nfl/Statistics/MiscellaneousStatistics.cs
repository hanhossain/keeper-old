using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
{
    public class MiscellaneousStatistics
    {
        public MiscellaneousStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_32").TextContent, out var conversions))
            {
                TwoPointConversions = conversions;
            }
        }
        
        public int? TwoPointConversions { get; }
    }
}