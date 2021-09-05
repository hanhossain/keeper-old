using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
{
    public class PlayerStatistics
    {
        public PlayerStatistics(IElement row, string position)
        {
            FantasyPoints = double.Parse(row.QuerySelector(".playerTotal").TextContent);

            switch (position)
            {
                case "DEF":
                    Defensive = new DefensiveStatistics(row);
                    break;
                case "K":
                    Kicking = new KickingStatistics(row);
                    break;
                default:
                    Offensive = new OffensiveStatistics(row);
                    break;
            }
        }
        
        public double FantasyPoints { get; }

        public OffensiveStatistics Offensive { get; }
        
        public DefensiveStatistics Defensive { get; }
        
        public KickingStatistics Kicking { get; }
    }
}