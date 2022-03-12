using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Keeper.Core.Sleeper
{
    public class SleeperPlayer
    {
        [JsonPropertyName("player_id")]
        public string PlayerId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        public string Position { get; set; }

        [JsonPropertyName("fantasy_positions")]
        public List<string> Positions { get; set; }

        public bool Active { get; set; }

        public string Team { get; set; }

        public string Status { get; set; }
    }
}
