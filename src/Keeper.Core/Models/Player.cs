namespace Keeper.Core.Models
{
    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string Team { get; set; }

        public string PositionAndTeam => $"{Position} - {Team}";
    }
}
