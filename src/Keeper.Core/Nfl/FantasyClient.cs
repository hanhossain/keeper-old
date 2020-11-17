using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;

namespace Keeper.Core.Nfl
{
    public class FantasyClient : IDisposable
    {
        private const int PageSize = 25;
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<NflPageResult> GetAsync(int season, int week, int offset, NflPosition? position = null)
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
                .Select(x => new NflPlayer(x))
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

        public async Task<List<NflPageResult>> GetAsync(int season, int week, NflPosition? position = null)
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

        public async Task<List<NflResult>> GetAsync(int season, NflPosition? position = null)
        {
            var week1Results = await GetAsync(season, 1, position);
            var weeks = week1Results.Select(x => x.Weeks).FirstOrDefault();

            var tasks = new List<Task<List<NflPageResult>>>();
            for (int i = 2; i <= weeks; i++)
            {
                tasks.Add(GetAsync(season, i, position));
            }

            var seasonResults = (await Task.WhenAll(tasks)).ToList();
            seasonResults.Insert(0, week1Results);

            var results = seasonResults
                .SelectMany(weekResults => weekResults)
                .GroupBy(
                    pageResult => pageResult.Week,
                    pageResult => pageResult.Values,
                    (week, players) => new NflResult()
                    {
                        Season = season,
                        Week = week,
                        Values = players.SelectMany(x => x).ToList()
                    })
                .ToList();

            return results;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}