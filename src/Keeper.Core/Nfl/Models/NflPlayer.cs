using System.Linq;
using AngleSharp.Dom;
using Keeper.Core.Nfl.Models.Statistics;

namespace Keeper.Core.Nfl.Models
{
    public class NflPlayer
    {
        public NflPlayer(IElement row, int season, int week)
        {
            Id = int.Parse(row.GetAttribute("class").Split(' ', '-')[1]);
            Name = row.QuerySelector(".playerName").TextContent;
            
            (Position, Team) = ParsePositionAndTeam(row);
            Statistics = new PlayerStatistics(row, Position);
            Season = season;
            Week = week;
        }
        
        public int Id { get; }

        public string Name { get; }

        public string Position { get; }

        public int Season { get; set; }

        public int Week { get; set; }
        
        public NflTeam Team { get; }

        public PlayerStatistics Statistics { get; }
        
        private (string, NflTeam) ParsePositionAndTeam(IElement row)
        {
            var positionAndTeamInfo = row.QuerySelector(".playerNameAndInfo em").TextContent.Split('-');
            var position = positionAndTeamInfo.First().Trim();

            NflTeam team = null;

            // defense doesn't have any team info
            if (positionAndTeamInfo.Length == 2)
            {
                var teamName = positionAndTeamInfo[1];
                var opponent = row.QuerySelector(".playerOpponent").TextContent;
                var location = opponent.StartsWith('@') ? NflLocation.Away : NflLocation.Home;
                opponent = opponent.TrimStart('@');

                team = new NflTeam()
                {
                    Name = teamName.Trim(),
                    Opponent = opponent.Trim(),
                    Location = location
                };
            }

            return (position, team);
        }
    }
}