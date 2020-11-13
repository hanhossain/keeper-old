using System;
namespace Keeper.Core.Models
{
    public class Player
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Position { get; set; }

        public string Team { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public string PositionAndTeam => $"{Position} - {Team}";
    }
}
