using System;
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
    public class PlayerService
    {
        private const int Season = 2020;

        private readonly AsyncLock _lock = new AsyncLock();
        private readonly Dictionary<int, Dictionary<int, PlayerStatistics>> _playerStatistics = new Dictionary<int, Dictionary<int, PlayerStatistics>>();
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();

        private bool _loaded = false;

        public async Task<List<Player>> GetPlayersAsync()
        {
            await LoadAsync();

            return _players.Values.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadAsync()
        {
            using var lease = await _lock.LockAsync();
            
            if (!_loaded)
            {
                using var sleeperClient = new SleeperClient();
                using var fantasyClient = new FantasyClient();

                var sleeperTask = sleeperClient.GetPlayersAsync();

                // get all statistics
                var tasks = new List<Task<List<NflResult>>>()
                {
                    fantasyClient.GetAsync(Season, NflPosition.Quarterback),
                    fantasyClient.GetAsync(Season, NflPosition.RunningBack),
                    fantasyClient.GetAsync(Season, NflPosition.WideReceiver),
                    fantasyClient.GetAsync(Season, NflPosition.TightEnd),
                    fantasyClient.GetAsync(Season, NflPosition.Kicker),
                    fantasyClient.GetAsync(Season, NflPosition.Defense)
                };

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
                                    team = players.Values.Where(x => player.Name == $"{x.FirstName} {x.LastName}").Select(x => x.Team).SingleOrDefault();
                                }

                                _players[player.Id] = new Player()
                                {
                                    Id = player.Id,
                                    Name = player.Name,
                                    Position = player.Position,
                                    Team = player.Team?.Name ?? team
                                };
                            }

                            if (!_playerStatistics.TryGetValue(player.Id, out var statistics))
                            {
                                statistics = new Dictionary<int, PlayerStatistics>();
                                _playerStatistics[player.Id] = statistics;
                            }

                            statistics[page.Week] = player.Statistics;
                        }
                    }
                }

                _loaded = true;
            }
        }
    }
}
