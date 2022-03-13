using System.Text.Json.Serialization;

namespace Keeper.Core.Sleeper.Models
{
    public class SleeperUser
    {
        public string Username { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("avatar")]
        public string AvatarId { get; set; }
    }
}
