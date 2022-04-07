using System.Collections.Generic;
using System.Linq;
using Keeper.Core.Database.Models;

namespace Keeper.Api.Models;

public class CalculatedOffensiveStatistics
{
    private CalculatedOffensiveStatistics(ICollection<NflOffensiveStatistics> offensiveStats)
    {
        FumblesLost = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.FumblesLost));
        FumbleTouchdowns = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.FumbleTouchdowns));
        PassingInterceptions = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.PassingInterceptions));
        PassingTouchdowns = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.PassingTouchdowns));
        PassingYards = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.PassingYards));
        ReceivingReceptions = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.ReceivingReceptions));
        ReceivingTouchdowns = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.ReceivingTouchdowns));
        ReceivingYards = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.ReceivingYards));
        ReturningTouchdowns = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.ReturningTouchdowns));
        RushingTouchdowns = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.RushingTouchdowns));
        RushingYards = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.RushingYards));
        TwoPointConversions = CalculatedStatistics.Calculate(offensiveStats.Select(x => x.TwoPointConversions));
    }

    public static CalculatedOffensiveStatistics Calculate(ICollection<NflOffensiveStatistics> offensiveStatistics)
    {
        if (offensiveStatistics == null)
        {
            return null;
        }

        var calculatedStats = new CalculatedOffensiveStatistics(offensiveStatistics);
        var properties = new[]
        {
            calculatedStats.FumblesLost,
            calculatedStats.FumbleTouchdowns,
            calculatedStats.PassingInterceptions,
            calculatedStats.PassingTouchdowns,
            calculatedStats.PassingYards,
            calculatedStats.ReceivingReceptions,
            calculatedStats.ReceivingTouchdowns,
            calculatedStats.ReceivingYards,
            calculatedStats.ReturningTouchdowns,
            calculatedStats.RushingTouchdowns,
            calculatedStats.RushingYards,
            calculatedStats.TwoPointConversions
        };

        return properties.Any(x => x != null) ? calculatedStats : null;
    }
    
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
