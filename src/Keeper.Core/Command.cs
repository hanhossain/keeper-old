using System.Linq;

namespace Keeper.Core
{
    public class Command
    {
        public Command(string[] args)
        {
            UpdateNfl = args.Contains("--update-nfl");
        }

        public bool UpdateNfl { get; }
    }
}
