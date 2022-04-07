using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Keeper.Api.Models;
using Keeper.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Keeper.Api.Controllers;

[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IKeeperRepository _keeperRepository;

    public PlayersController(IKeeperRepository keeperRepository)
    {
        _keeperRepository = keeperRepository;
    }

    [HttpGet]
    [Route("api/players")]
    public async Task<IActionResult> GetPlayers(CancellationToken cancellationToken, string query = null)
    {
        var dbPlayers = await _keeperRepository.GetPlayersAsync(query, cancellationToken);

        var players = dbPlayers
            .Select(x => new Player()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FullName = x.FullName,
                Position = x.Position,
                Team = x.Team
            })
            .ToList();
        return Ok(players);
    }

    [HttpGet]
    [Route("api/players/{playerId}")]
    public async Task<IActionResult> GetPlayer(string playerId, CancellationToken cancellationToken)
    {
        var dbPlayer = await _keeperRepository.GetPlayerAsync(playerId, cancellationToken);

        if (dbPlayer == null)
        {
            return NotFound();
        }

        var player = new Player()
        {
            Id = dbPlayer.Id,
            FirstName = dbPlayer.FirstName,
            LastName = dbPlayer.LastName,
            FullName = dbPlayer.FullName,
            Position = dbPlayer.Position,
            Team = dbPlayer.Team
        };

        return Ok(player);
    }

    [HttpGet]
    [Route("api/players/{playerId}/seasons/{season}")]
    public async Task<IActionResult> GetSeasonStatistics(string playerId, int season, CancellationToken cancellationToken)
    {
        var dbPlayer = await _keeperRepository.GetPlayerSeasonStatisticsAsync(playerId, season, cancellationToken);
        
        if (dbPlayer == null)
        {
            return NotFound();
        }
        
        // TODO: this should have it's own model. It also needs to have all data (from offensive, defensive, and kicking), not just passing and rushing data.
        var result = new
        {
            FantasyPoints = dbPlayer.PlayerStatistics.Sum(x => x.FantasyPoints),
            PassingInterceptions = dbPlayer.NflOffensiveStatistics.Sum(x => x.PassingInterceptions),
            PassingTouchdowns = dbPlayer.NflOffensiveStatistics.Sum(x => x.PassingTouchdowns),
            PassingYards = dbPlayer.NflOffensiveStatistics.Sum(x => x.PassingYards),
            RushingTouchdowns = dbPlayer.NflOffensiveStatistics.Sum(x => x.RushingTouchdowns),
            RushingYards = dbPlayer.NflOffensiveStatistics.Sum(x => x.RushingYards)
        };
        return Ok(result);
    }
}
