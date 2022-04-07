namespace Keeper.Api.Models;

public class CalculatedOffensiveStatistics
{
    public CalculatedStatistics PassingYards { get; set; }

    public CalculatedStatistics PassingTouchdowns { get; set; }

    public CalculatedStatistics PassingInterceptions { get; set; }

    public CalculatedStatistics RushingYards { get; set; }

    public CalculatedStatistics RushingTouchdowns { get; set; }

    public CalculatedStatistics ReceivingReceptions { get; set; }

    public CalculatedStatistics ReceivingYards { get; set; }

    public CalculatedStatistics ReceivingTouchdowns { get; set; }

    public CalculatedStatistics ReturningTouchdowns { get; set; }

    public CalculatedStatistics TwoPointConversions { get; set; }

    public CalculatedStatistics FumbleTouchdowns { get; set; }

    public CalculatedStatistics FumblesLost { get; set; }
}
