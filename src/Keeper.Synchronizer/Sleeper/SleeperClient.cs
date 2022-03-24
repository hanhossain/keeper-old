using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Keeper.Synchronizer.Sleeper.Models;

namespace Keeper.Synchronizer.Sleeper
{
    public sealed class SleeperClient : ISleeperClient
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

        public async Task<SleeperNflState> GetNflStateAsync()
        {
            using var response = await _client.GetAsync("https://api.sleeper.app/v1/state/nfl");
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<SleeperNflState>(stream, _options);
        }

        public async Task<SleeperUser> GetUserAsync(string usernameOrId)
        {
            using var response = await _client.GetAsync($"https://api.sleeper.app/v1/user/{usernameOrId}");
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<SleeperUser>(stream, _options);
        }

        public async Task<byte[]> GetAvatarAsync(string avatarId)
        {
            using var response = await _client.GetAsync($"https://sleepercdn.com/avatars/{avatarId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetAvatarThumbnailAsync(string avatarId)
        {
            using var response = await _client.GetAsync($"https://sleepercdn.com/avatars/thumbs/{avatarId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
