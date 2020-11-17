using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Keeper.Core.Delegates;
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

        public async Task<List<Player>> GetPlayersAsync(IProgressDelegate progressDelegate)
        {
            await LoadAsync(progressDelegate);

            return _players.Values.OrderBy(x => x.Name).ToList();
        }

        public async Task LoadAsync(IProgressDelegate progressDelegate)
        {
            using var lease = await _lock.LockAsync();
            
            if (!_loaded)
            {
                progressDelegate.ShowProgressIndicator();

                using var sleeperClient = new SleeperClient();
                using var fantasyClient = new FantasyClient();

                int finishedCount = 0;
                int totalParallelTasks = 7;
                var innerLock = new AsyncLock();

                async Task<Dictionary<string, SleeperPlayer>> GetPlayersAsync()
                {
                    using var sleeperClient = new SleeperClient();
                    var result = await sleeperClient.GetPlayersAsync();

                    using var innerLease = await innerLock.LockAsync();
                    finishedCount++;

                    progressDelegate.UpdateProgress((float)finishedCount / totalParallelTasks);

                    return result;
                }

                async Task<List<NflResult>> GetStatisticsAsync(NflPosition position)
                {
                    var result = await fantasyClient.GetAsync(Season, position);
                    using var innerLease = await innerLock.LockAsync();
                    finishedCount++;

                    progressDelegate.UpdateProgress((float)finishedCount / totalParallelTasks);

                    return result;
                }

                // get all statistics
                var tasks = new List<Task<List<NflResult>>>()
                {
                    GetStatisticsAsync(NflPosition.Quarterback),
                    GetStatisticsAsync(NflPosition.RunningBack),
                    GetStatisticsAsync(NflPosition.WideReceiver),
                    GetStatisticsAsync(NflPosition.TightEnd),
                    GetStatisticsAsync(NflPosition.Kicker),
                    GetStatisticsAsync(NflPosition.Defense)
                };

                var sleeperTask = GetPlayersAsync();
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
                progressDelegate.DismissProgressIndicator();
            }
        }
    }
}
