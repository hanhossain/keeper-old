using AngleSharp.Dom;

namespace Keeper.ConsoleApp.Nfl.Statistics
{
    public class DefensiveStatistics
    {
        public DefensiveStatistics(IElement row)
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