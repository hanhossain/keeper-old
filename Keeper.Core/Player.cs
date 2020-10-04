using System;
using System.Linq;
using AngleSharp.Dom;
using Keeper.Core.Statistics;

namespace Keeper.Core
{
    public class Player
    {
        public Player(IElement row)
        {
            Id = int.Parse(row.GetAttribute("class").Split(' ', '-')[1]);
            Name = row.QuerySelector(".playerName").TextContent;
            
            (Position, Team) = ParsePositionAndTeam(row);

            Statistics = Position switch
            {
                Position.Kicker => new KickingStatistics(row),
                Position.Defense => throw new NotImplementedException(),
                _ => new OffensiveStatistics(row)
            };
        }
        
        public int Id { get; }

        public string Name { get; }

        public Position Position { get; }

        public Team Team { get; }

        public PlayerStatistics Statistics { get; }
        
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