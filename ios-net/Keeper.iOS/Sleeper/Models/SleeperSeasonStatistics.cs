using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Keeper.iOS.Sleeper.Models;

public class SleeperSeasonStatistics
{
    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; }

    public int Season { get; set; }

    public Dictionary<string, double> Stats { get; set; }
}
