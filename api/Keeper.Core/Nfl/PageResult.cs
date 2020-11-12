using System.Collections.Generic;

namespace Keeper.Core.Nfl
{
    public class PageResult<T>
    {
        public List<T> Values { get; set; }
        
        public int TotalCount { get; set; }
    }
}