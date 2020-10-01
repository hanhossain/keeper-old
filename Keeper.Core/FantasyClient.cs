using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;

namespace Keeper.Core
{
    public class FantasyClient
    {
        private readonly HttpClient _httpClient;

        public FantasyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAsync(Position position, int season, int week, int offset)
        {
            string uri = $"https://fantasy.nfl.com/research/players?offset={offset}&position={(int)position}" +
                         $"&sort=pts&statCategory=stats&statSeason={season}&statType=weekStats&statWeek={week}";

            return await _httpClient.GetStringAsync(uri);
        }

        public async Task<int> TotalPlayersAsync(Position position, int season, int week, int offset)
        {
            string uri = $"https://fantasy.nfl.com/research/players?offset={offset}&position={(int)position}" +
                         $"&sort=pts&statCategory=stats&statSeason={season}&statType=weekStats&statWeek={week}";

            await using var stream = await _httpClient.GetStreamAsync(uri);
            
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(x => x.Content(stream));

            var cell = document.QuerySelector(".paginationTitle");
            return int.Parse(cell.TextContent.Trim().Split().Last());
        }
    }
}