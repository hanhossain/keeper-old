using System.Linq;

namespace Keeper.Core
{
    public class Command
    {
        public Command(string[] args)
        {
            UpdateNfl = args.Contains("--update-nfl");
            UpdateSleeper = args.Contains("--update-sleeper");
        }

        public bool UpdateNfl { get; }

        public bool UpdateSleeper { get; }
    }
}
