using AngleSharp.Dom;

namespace Keeper.Synchronizer.Nfl.Models.Statistics
{
    public class ScoreStatistics
    {
        public ScoreStatistics(IElement row)
        {
            if (int.TryParse(row.QuerySelector(".stat_49").TextContent, out var safeties))
            {
                Safeties = safeties;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_50").TextContent, out var touchdowns))
            {
                Touchdowns = touchdowns;
            }
            
            if (int.TryParse(row.QuerySelector(".stat_93").TextContent, out var def2PtRet))
            {
                Def2PtRet = def2PtRet;
            }
        }
        
        public int? Safeties { get; }
        
        public int? Touchdowns { get; }
        
        public int? Def2PtRet { get; }
    }
}