using AngleSharp.Dom;

namespace Keeper.Core
{
    public abstract class Statistics
    {
        protected Statistics(IElement row)
        {
            FantasyPoints = double.Parse(row.QuerySelector(".playerTotal").TextContent);
        }
        
        public double FantasyPoints { get; }
    }
}