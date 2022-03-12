using System.Text.Json.Serialization;

namespace Keeper.Core.Sleeper
{
    public class SleeperNflState
    {
        public int Week { get; set; }

        /// <summary>
        /// Season types: <code>[pre, post, regular]</code>
        /// </summary>
        [JsonPropertyName("season_type")]
        public string SeasonType { get; set; }

        /// <summary>
        /// Regular season start
        /// </summary>
        [JsonPropertyName("season_start_date")]
        public string SeasonStartDate { get; set; }

        /// <summary>
        /// Current season
        /// </summary>
        public int Season { get; set; }

        [JsonPropertyName("previous_season")]
        public int PreviousSeason { get; set; }

        /// <summary>
        /// Week of regular season
        /// </summary>
        public int Leg { get; set; }


        /// <summary>
        /// Active season for leagues
        /// </summary>
        [JsonPropertyName("league_season")]
        public int LeagueSeason { get; set; }

        /// <summary>
        /// Flips in December
        /// </summary>
        [JsonPropertyName("league_create_season")]
        public int LeagueCreateSeason { get; set; }

        /// <summary>
        /// Which week to display in UI, can be different than week
        /// </summary>
        [JsonPropertyName("display_week")]
        public int DisplayWeek { get; set; }
    }
}
