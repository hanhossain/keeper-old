using System;

namespace Keeper.iOS
{
    public interface IUserDefaults
    {
        DateTime SleeperLastUpdated { get; set; }
    }
}
