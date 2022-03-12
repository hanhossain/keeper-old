using System.Collections.Generic;

namespace Keeper.ConsoleApp.Nfl
{
    public class NflResult
    {
        public int Season { get; set; }

        public int Week { get; set; }

        public List<NflPlayer> Values { get; set; }
    }
}
