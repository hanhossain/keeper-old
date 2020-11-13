using System.Collections.Generic;
using Keeper.Core.Nfl;

namespace Keeper.Core
{
    public class NflResult
    {
        public int Season { get; set; }

        public int Week { get; set; }

        public List<NflPlayer> Values { get; set; }
    }
}
