namespace Keeper.Synchronizer.Nfl.Models
{
    public class NflTeam
    {
        public string Name { get; set; }

        public string Opponent { get; set; }

        public NflLocation Location { get; set; }
    }
}