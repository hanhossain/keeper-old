using System.Collections.Generic;
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
            try
            {
                var player = await _playerService.GetPlayerAsync(playerId);
                return Ok(player);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/players/{playerId}/seasons/{season}/statistics")]
        public async Task<IActionResult> GetPlayerStatistics(int playerId, int season)
        {
            try
            {
                var stats = await _playerService.GetPlayerStatisticsAsync(playerId, season);
                return Ok(stats);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("api/players/{playerId}/seasons/{season}/weeks/{week}/matchups")]
        public async Task<IActionResult> GetPlayerMatchups(int playerId, int season, int week)
        {
            try
            {
                var matchups = await _playerService.GetPlayerMatchupsAsync(playerId, season, week);
                return Ok(matchups);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
