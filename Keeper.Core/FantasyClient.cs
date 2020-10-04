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

        public async Task<PageResult<Player>> GetAsync(Position position, int season, int week, int offset)
        {
            string uri = $"https://fantasy.nfl.com/research/players?offset={offset}&position={(int)position}" +
                         $"&sort=pts&statCategory=stats&statSeason={season}&statType=weekStats&statWeek={week}";

            await using var stream = await _httpClient.GetStreamAsync(uri);
            
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(x => x.Content(stream));

            var rawTotalCount = document
                .QuerySelector(".paginationTitle")
                .TextContent
                .Trim()
                .Split()
                .Last();
            var totalCount = int.Parse(rawTotalCount);
            
            var tableBody = document.QuerySelector("table tbody");

            var players = tableBody
                .QuerySelectorAll("tr")
                .Select(x => new Player(x))
                .ToList();

            return new PageResult<Player>()
            {
                TotalCount = totalCount,
                Values = players
            };
        }
    }
}