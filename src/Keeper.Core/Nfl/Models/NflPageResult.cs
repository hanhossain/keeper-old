using System.Collections.Generic;

namespace Keeper.Core.Nfl.Models
{
    public class NflPageResult
    {
        public int Season { get; set; }

        public int Week { get; set; }

        public List<NflPlayer> Values { get; set; }
        
        public int TotalCount { get; set; }

        public int Weeks { get; set; }
    }
}