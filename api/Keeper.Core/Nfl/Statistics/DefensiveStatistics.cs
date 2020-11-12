using AngleSharp.Dom;

namespace Keeper.Core.Nfl.Statistics
{
    public class DefensiveStatistics : PlayerStatistics
    {
        public DefensiveStatistics(IElement row)
            : base(row)
        {
            Tackling = new TacklingStatistics(row);
            Turnover = new TurnoverStatistics(row);
            Score = new ScoreStatistics(row);
            Returning = new ReturningStatistics(row, false);
            Points = new PointsStatistics(row);
        }

        public TacklingStatistics Tackling { get; }

        public TurnoverStatistics Turnover { get; }

        public ScoreStatistics Score { get; }

        public ReturningStatistics Returning { get; }

        public PointsStatistics Points { get; }
    }
}