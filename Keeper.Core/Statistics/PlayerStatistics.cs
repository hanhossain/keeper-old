using AngleSharp.Dom;

namespace Keeper.Core.Statistics
{
    public abstract class PlayerStatistics
    {
        protected PlayerStatistics(IElement row)
        {
            FantasyPoints = double.Parse(row.QuerySelector(".playerTotal").TextContent);
        }
        
        public double FantasyPoints { get; }
    }
}