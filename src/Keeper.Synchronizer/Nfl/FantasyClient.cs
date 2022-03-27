using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Keeper.Synchronizer.Nfl.Models;

namespace Keeper.Synchronizer.Nfl
{
    // TODO: propagate cancellation tokens
    public class FantasyClient : IFantasyClient
    {
        private const int PageSize = 25;
        private readonly HttpClient _httpClient;

        public FantasyClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NflPageResult> GetAsync(int season, int week, int offset, NflPosition position)
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

            var players = tableBody?
                .QuerySelectorAll("tr")
                .Select(x => new NflPlayer(x, season, week))
                .ToList() ?? new List<NflPlayer>();

            var weeks = document
                .QuerySelectorAll(".ww")
                .Count();

            return new NflPageResult()
            {
                Season = season,
                Week = week,
                TotalCount = totalCount,
                Values = players,
                Weeks = weeks
            };
        }

        public async Task<List<NflPageResult>> GetAsync(int season, int week, NflPosition position)
        {
            var week1Results = await GetAsync(season, week, 1, position);

            var tasks = new List<Task<NflPageResult>>();
            
            for (int i = PageSize + 1; i <= week1Results.TotalCount; i += PageSize)
            {
                var task = GetAsync(season, week, i, position);
                tasks.Add(task);
            }

            var results = await Task.WhenAll(tasks);
            var players = new List<NflPageResult>() { week1Results };

            players.AddRange(results);

            return players;
        }

        public async Task<List<NflPlayer>> GetAsync(int season, int week)
        {
            var positions = new[]
            {
                NflPosition.Quarterback,
                NflPosition.RunningBack,
                NflPosition.WideReceiver,
                NflPosition.TightEnd,
                NflPosition.Kicker,
                NflPosition.Defense
            };

            var tasks = positions.Select(x => GetAsync(season, week, x));
            var weekResults = await Task.WhenAll(tasks);
            return weekResults
                .SelectMany(x => x)
                .SelectMany(x => x.Values)
                .ToList();
        }
    }
}