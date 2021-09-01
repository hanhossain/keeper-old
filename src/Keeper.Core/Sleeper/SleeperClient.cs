using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Keeper.Core.Sleeper
{
    public sealed class SleeperClient : ISleeperClient
    {
        private readonly HttpClient _client;

        public SleeperClient(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public async Task<Dictionary<string, SleeperPlayer>> GetPlayersAsync()
        {
            using var response = await _client.GetAsync("https://api.sleeper.app/v1/players/nfl");
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return await JsonSerializer.DeserializeAsync<Dictionary<string, SleeperPlayer>>(stream, options);
        }
    }
}
