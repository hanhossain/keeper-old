using System.Collections.Generic;
using System.Threading.Tasks;
using Keeper.Synchronizer.Sleeper.Models;

namespace Keeper.Synchronizer.Sleeper
{
    public interface ISleeperClient
    {
        Task<Dictionary<string, SleeperPlayer>> GetPlayersAsync();

        Task<SleeperNflState> GetNflStateAsync();

        Task<SleeperUser> GetUserAsync(string usernameOrId);

        Task<byte[]> GetAvatarAsync(string avatarId);

        Task<byte[]> GetAvatarThumbnailAsync(string avatarId);
    }
}
