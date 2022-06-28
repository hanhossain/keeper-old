using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Keeper.iOS.Sleeper.Models;

public class SleeperPlayerStatistics
{
    public int Season { get; set; }

    public int Week { get; set; }

    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; }

    public string Team { get; set; }

    public string Opponent { get; set; }

    [JsonPropertyName("game_id")]
    public string GameId { get; set; }

    public string Date { get; set; }

    public Dictionary<string, double> Stats { get; set; }
}
