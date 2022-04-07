using System.Collections.Generic;
using System.Linq;
using Keeper.Core.Database.Models;

namespace Keeper.Api.Models;

public class CalculatedDefensiveStatistics
{
    public CalculatedDefensiveStatistics(ICollection<NflDefensiveStatistics> defensiveStats)
    {
        Interceptions = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.Interceptions));
        Sacks = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.Sacks));
        Safeties = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.Safeties));
        Touchdowns = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.Touchdowns));
        FumblesRecovered = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.FumblesRecovered));
        PointsAllowed = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.PointsAllowed));
        RetTouchdowns = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.RetTouchdowns));
        Def2PtRet = CalculatedStatistics.Calculate(defensiveStats.Select(x => x.Def2PtRet));
    }

    public static CalculatedDefensiveStatistics Calculate(ICollection<NflDefensiveStatistics> defensiveStats)
    {
        if (defensiveStats == null)
        {
            return null;
        }

        var calculatedStats = new CalculatedDefensiveStatistics(defensiveStats);
        var properties = new[]
        {
            calculatedStats.Interceptions,
            calculatedStats.Sacks,
            calculatedStats.Safeties,
            calculatedStats.Touchdowns,
            calculatedStats.FumblesRecovered,
            calculatedStats.PointsAllowed,
            calculatedStats.RetTouchdowns,
            calculatedStats.Def2PtRet
        };

        return properties.Any(x => x != null) ? calculatedStats : null;
    }
    
    public CalculatedStatistics Sacks { get; set; }

    public CalculatedStatistics Interceptions { get; set; }

    public CalculatedStatistics FumblesRecovered { get; set; }

    public CalculatedStatistics Safeties { get; set; }

    public CalculatedStatistics Touchdowns { get; set; }

    public CalculatedStatistics Def2PtRet { get; set; }

    public CalculatedStatistics RetTouchdowns { get; set; }

    public CalculatedStatistics PointsAllowed { get; set; }
}
