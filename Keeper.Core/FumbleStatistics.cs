using AngleSharp.Dom;

namespace Keeper.Core
{
    public class FumbleStatistics
    {
        public FumbleStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_29").TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_30").TextContent, out var lost))
            {
                Lost = lost;
            }
        }
        
        public int? Touchdowns { get; }
        public int? Lost { get; }
    }
}