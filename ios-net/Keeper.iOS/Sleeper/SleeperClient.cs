using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Keeper.iOS.Sleeper.Models;

namespace Keeper.iOS.Sleeper;

public class SleeperClient
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public SleeperClient(HttpClient httpClient)
    {
        _client = httpClient;
    }

    public async Task<Dictionary<string, SleeperPlayer>> GetPlayersAsync()
    {
        using var response = await _client.GetAsync("https://api.sleeper.app/v1/players/nfl");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<Dictionary<string, SleeperPlayer>>(stream, _options);
    }

    public async Task<List<SleeperSeasonStatistics>> GetSeasonStatisticsAsync()
    {
        using var response = await _client.GetAsync("https://api.sleeper.com/stats/nfl/2021?season_type=regular");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<List<SleeperSeasonStatistics>>(stream, _options);
    }

    public async Task<byte[]> GetAvatarAsync(string playerId)
    {
        var uri = char.IsLetter(playerId[0])
            ? $"https://sleepercdn.com/images/team_logos/nfl/{playerId.ToLower()}.png"
            : $"https://sleepercdn.com/content/nfl/players/{playerId}.jpg";
        using var response = await _client.GetAsync(uri);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

        return null;
    }
}
