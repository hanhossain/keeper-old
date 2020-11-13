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
                NflPosition.Kicker => new KickingStatistics(row),
                NflPosition.Defense => new DefensiveStatistics(row),
                _ => new OffensiveStatistics(row)
            };
        }
        
        public int Id { get; }

        public string Name { get; }

        public NflPosition Position { get; }
        
        public NflTeam Team { get; }

        public PlayerStatistics Statistics { get; }
        
        private (NflPosition, NflTeam) ParsePositionAndTeam(IElement row)
        {
            var positionAndTeamInfo = row.QuerySelector(".playerNameAndInfo em").TextContent.Split('-');
            var position = positionAndTeamInfo.First().Trim() switch
            {
                "QB" => NflPosition.Quarterback,
                "RB" => NflPosition.RunningBack,
                "WR" => NflPosition.WideReceiver,
                "TE" => NflPosition.TightEnd,
                "K" => NflPosition.Kicker,
                "DEF" => NflPosition.Defense,
                _ => throw new ArgumentException("Invalid position")
            };

            NflTeam team = null;

            if (positionAndTeamInfo.Length == 2)
            {
                // this doesn't work for defense
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