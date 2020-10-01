namespace Keeper.Core
{
    public class Player
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Position Position { get; set; }

        public Team Team { get; set; }

        public double Points { get; set; }

        public PassingStatistics Passing { get; set; }
    }
}