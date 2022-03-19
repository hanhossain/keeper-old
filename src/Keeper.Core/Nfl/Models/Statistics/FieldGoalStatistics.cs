using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Models.Statistics
{
    public class FieldGoalStatistics
    {
        public FieldGoalStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_35").TextContent, out var yards0To19))
            {
                Yards0To19 = yards0To19;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_36").TextContent, out var yards20To29))
            {
                Yards20To29 = yards20To29;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_37").TextContent, out var yards30To39))
            {
                Yards30To39 = yards30To39;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_38").TextContent, out var yards40To49))
            {
                Yards40To49 = yards40To49;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_39").TextContent, out var yards50Plus))
            {
                Yards50Plus = yards50Plus;
            }
        }
        
        public int? Yards0To19 { get; }
        
        public int? Yards20To29 { get; }
        
        public int? Yards30To39 { get; }
        
        public int? Yards40To49 { get; }
        
        public int? Yards50Plus { get; }
    }
}