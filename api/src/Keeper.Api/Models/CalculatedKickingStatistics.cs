using System.Collections.Generic;
using System.Linq;
using Keeper.Core.Database.Models;

namespace Keeper.Api.Models;

public class CalculatedKickingStatistics
{
    private CalculatedKickingStatistics(ICollection<NflKickingStatistics> kickingStats)
    {
        PatMade = CalculatedStatistics.Calculate(kickingStats.Select(x => x.PatMade));
        FieldGoal0To19Yards = CalculatedStatistics.Calculate(kickingStats.Select(x => x.FieldGoal0To19Yards));
        FieldGoal20To29Yards = CalculatedStatistics.Calculate(kickingStats.Select(x => x.FieldGoal20To29Yards));
        FieldGoal30To39Yards = CalculatedStatistics.Calculate(kickingStats.Select(x => x.FieldGoal30To39Yards));
        FieldGoal40To49Yards = CalculatedStatistics.Calculate(kickingStats.Select(x => x.FieldGoal40To49Yards));
        FieldGoal50PlusYards = CalculatedStatistics.Calculate(kickingStats.Select(x => x.FieldGoal50PlusYards));
    }

    public static CalculatedKickingStatistics Calculate(ICollection<NflKickingStatistics> kickingStats)
    {
        if (kickingStats == null)
        {
            return null;
        }

        var calculatedStats = new CalculatedKickingStatistics(kickingStats);
        var properties = new[]
        {
            calculatedStats.PatMade,
            calculatedStats.FieldGoal0To19Yards,
            calculatedStats.FieldGoal20To29Yards,
            calculatedStats.FieldGoal30To39Yards,
            calculatedStats.FieldGoal40To49Yards,
            calculatedStats.FieldGoal50PlusYards,
        };

        return properties.Any(x => x != null) ? calculatedStats : null;
    }

    public CalculatedStatistics PatMade { get; }
        
    public CalculatedStatistics FieldGoal0To19Yards { get; }
        
    public CalculatedStatistics FieldGoal20To29Yards { get; }
        
    public CalculatedStatistics FieldGoal30To39Yards { get; }
        
    public CalculatedStatistics FieldGoal40To49Yards { get; }
        
    public CalculatedStatistics FieldGoal50PlusYards { get; }
}
