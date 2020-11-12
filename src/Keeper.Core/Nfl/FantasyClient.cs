using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;

namespace Keeper.Core.Nfl
{
    public class FantasyClient
    {
        private const int PageSize = 25;
        private readonly HttpClient _httpClient;

        public FantasyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PageResult<Player>> GetAsync(int season, int week, int offset, Position? position = null)
        {
            string positionQuery = position != null ? $"&position={(int)position}" : string.Empty;
            
            string uri = $"https://fantasy.nfl.com/research/players?offset={offset}{positionQuery}" +
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

            var players = tableBody?
                .QuerySelectorAll("tr")
                .Select(x => new Player(x))
                .ToList() ?? new List<Player>();

            return new PageResult<Player>()
            {
                TotalCount = totalCount,
                Values = players
            };
        }

        public async Task<List<Player>> GetAsync(int season, int week, Position? position = null)
        {
            var week1Results = await GetAsync(season, week, 1, position);

            var tasks = new List<Task<PageResult<Player>>>();
            
            for (int i = PageSize + 1; i <= week1Results.TotalCount; i += PageSize)
            {
                var task = GetAsync(season, week, i, position);
                tasks.Add(task);
            }

            var results = await Task.WhenAll(tasks);
            var players = new List<Player>();

            players.AddRange(week1Results.Values);

            foreach (var weekResults in results)
            {
                players.AddRange(weekResults.Values);
            }

            return players;
        }
    }
}