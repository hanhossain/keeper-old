namespace Keeper.Api.Models;

public class PlayerSeasonStatistics
{
    public CalculatedStatistics FantasyPoints { get; set; }
    
    public CalculatedOffensiveStatistics Offensive { get; set; }

    public CalculatedDefensiveStatistics Defensive { get; set; }
}
