using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.Core.Models;
using Keeper.Core.Nfl;
using Keeper.Core.Nfl.Statistics;
using Keeper.Core.Sleeper;
using Nito.AsyncEx;

namespace Keeper.Core.Services
{
    public class PlayerService : IPlayerService
    {
        private const int Season = 2020;

        private readonly AsyncLock _lock = new();
        
        // { playerId: { season: { week: statistics }}}
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, PlayerStatistics>>> _playerStatistics = new();
        private readonly Dictionary<int, Player> _players = new();
        
        // { team: { season: { week: opponent }}}
        private readonly Dictionary<string, Dictionary<int, Dictionary<int, string>>> _matchups = new(); 
        
        private readonly ISleeperClient _sleeperClient;
        private readonly IFantasyClient _fantasyClient;
        
        private bool _loaded;

        public PlayerService(ISleeperClient sleeperClient, IFantasyClient fantasyClient)
        {
            _sleeperClient = sleeperClient;
            _fantasyClient = fantasyClient;
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            await LoadAsync(Season);

            return _players.Values.OrderBy(x => x.Name).ToList();
        }

        public async Task<Player> GetPlayerAsync(int playerId)
        {
            await LoadAsync(Season);

            return _players[playerId];
        }

        public async Task<Dictionary<int, PlayerStatistics>> GetPlayerStatisticsAsync(int playerId, int season)
        {
            await LoadAsync(season);
            return _playerStatistics[playerId][season];
        }

        public async Task<List<PlayerMatchup>> GetPlayerMatchupsAsync(int playerId, int season, int week)
        {
            await LoadAsync(season);
            var playerTeam = _players[playerId].Team;
            var opponent = _matchups[playerTeam][season][week];

            return _players
                .Values
                .Where(x => x.Team == opponent)
                .Select(x => new PlayerMatchup()
                {
                    Player = x,
                    Statistics = _playerStatistics[x.Id][season][week]
                })
                .ToList();
        }
        
        private async Task LoadAsync(int season)
        {
            using var lease = await _lock.LockAsync();
            
            if (!_loaded)
            {
                // get all statistics
                var tasks = new List<Task<List<NflResult>>>()
                {
                    _fantasyClient.GetAsync(season, NflPosition.Quarterback),
                    _fantasyClient.GetAsync(season, NflPosition.RunningBack),
                    _fantasyClient.GetAsync(season, NflPosition.WideReceiver),
                    _fantasyClient.GetAsync(season, NflPosition.TightEnd),
                    _fantasyClient.GetAsync(season, NflPosition.Kicker),
                    _fantasyClient.GetAsync(season, NflPosition.Defense)
                };

                var sleeperTask = _sleeperClient.GetPlayersAsync();
                var results = await Task.WhenAll(tasks);
                var players = await sleeperTask;

                foreach (var result in results)
                {
                    foreach (var page in result)
                    {
                        foreach (var player in page.Values)
                        {
                            if (!_players.ContainsKey(player.Id))
                            {
                                string team = null;

                                if (player.Position == "DEF")
                                {
                                    team = players.Values
                                        .Where(x => player.Name == $"{x.FirstName} {x.LastName}")
                                        .Select(x => x.Team)
                                        .SingleOrDefault();
                                }

                                var playerModel = new Player()
                                {
                                    Id = player.Id,
                                    Name = player.Name,
                                    Position = player.Position,
                                    Team = player.Team?.Name ?? team
                                };
                                
                                _players[player.Id] = playerModel;

                                if (!string.IsNullOrWhiteSpace(playerModel.Team))
                                {
                                    if (!_matchups.TryGetValue(playerModel.Team, out var teamMatchups))
                                    {
                                        teamMatchups = new Dictionary<int, Dictionary<int, string>>();
                                        _matchups[playerModel.Team] = teamMatchups;
                                    }

                                    if (!teamMatchups.TryGetValue(season, out var seasonMatchups))
                                    {
                                        seasonMatchups = new Dictionary<int, string>();
                                        teamMatchups[season] = seasonMatchups;
                                    }

                                    if (!string.IsNullOrWhiteSpace(player.Team?.Opponent) && !seasonMatchups.ContainsKey(page.Week))
                                    {
                                        seasonMatchups[page.Week] = player.Team?.Opponent;
                                    }
                                }
                            }

                            if (!_playerStatistics.TryGetValue(player.Id, out var statistics))
                            {
                                statistics = new Dictionary<int, Dictionary<int, PlayerStatistics>>();
                                _playerStatistics[player.Id] = statistics;
                            }

                            if (!statistics.TryGetValue(season, out var seasonStatistics))
                            {
                                seasonStatistics = new Dictionary<int, PlayerStatistics>();
                                statistics[season] = seasonStatistics;
                            }
                            
                            seasonStatistics[page.Week] = player.Statistics;
                        }
                    }
                }

                _loaded = true;
            }
        }
    }
}
