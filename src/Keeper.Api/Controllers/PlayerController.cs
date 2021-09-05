using System.Threading.Tasks;
using Keeper.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Keeper.Api.Controllers
{
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        
        [HttpGet]
        [Route("api/players")]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _playerService.GetPlayersAsync();
            return Ok(players);
        }

        [HttpGet]
        [Route("api/players/{playerId}")]
        public async Task<IActionResult> GetPlayer(int playerId)
        {
            var player = await _playerService.GetPlayerAsync(playerId);
            return Ok(player);
        }

        [HttpGet]
        [Route("api/players/{playerId}/statistics")]
        public async Task<IActionResult> GetPlayerStatistics(int playerId)
        {
            var stats = await _playerService.GetPlayerStatistics(playerId);
            return Ok(stats);
        }
    }
}
