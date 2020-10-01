using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

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

            var players = tableBody.QuerySelectorAll("tr").Select(ParsePlayer).ToList();

            return new PageResult<Player>()
            {
                TotalCount = totalCount,
                Values = players
            };
        }

        private Player ParsePlayer(IElement row)
        {
            var id = int.Parse(row.GetAttribute("class").Split(' ', '-')[1]);
            var name = row.QuerySelector(".playerName").TextContent;
            
            var (position, team) = ParsePositionAndTeam(row);

            var points = double.Parse(row.QuerySelector(".playerTotal").TextContent);

            return new Player
            {
                Id = id,
                Name = name,
                Position = position,
                Team = team,
                Points = points
            };
        }

        private (Position, Team) ParsePositionAndTeam(IElement row)
        {
            var positionAndTeamInfo = row.QuerySelector(".playerNameAndInfo em").TextContent.Split('-');
            var position = positionAndTeamInfo.First().Trim() switch
            {
                "QB" => Position.Quarterback,
                "RB" => Position.RunningBack,
                "WR" => Position.WideReceiver,
                "TE" => Position.TightEnd,
                "K" => Position.Kicker,
                "DEF" => Position.Defense,
                _ => throw new ArgumentException("Invalid position")
            };

            Team team = null;

            if (positionAndTeamInfo.Length == 2)
            {
                var teamName = positionAndTeamInfo[1];
                var opponent = row.QuerySelector(".playerOpponent").TextContent;
                var location = opponent.StartsWith('@') ? Location.Away : Location.Home;
                opponent = opponent.TrimStart('@');

                team = new Team()
                {
                    Name = teamName,
                    Opponent = opponent,
                    Location = location
                };
            }

            return (position, team);
        }
    }
}