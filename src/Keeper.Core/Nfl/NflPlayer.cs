using System;
using System.Linq;
using AngleSharp.Dom;
using Keeper.Core.Nfl.Statistics;

namespace Keeper.Core.Nfl
{
    public class NflPlayer
    {
        public NflPlayer(IElement row)
        {
            Id = int.Parse(row.GetAttribute("class").Split(' ', '-')[1]);
            Name = row.QuerySelector(".playerName").TextContent;
            
            (Position, Team) = ParsePositionAndTeam(row);

            Statistics = Position switch
            {
                "K" => new KickingStatistics(row),
                "DEF" => new DefensiveStatistics(row),
                _ => new OffensiveStatistics(row)
            };
        }
        
        public int Id { get; }

        public string Name { get; }

        public string Position { get; }
        
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