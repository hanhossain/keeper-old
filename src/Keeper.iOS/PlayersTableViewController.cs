using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Keeper.Core.Database;
using Keeper.Core.Sleeper;
using Microsoft.EntityFrameworkCore;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersTableViewController : UITableViewController
    {
        private const string CellId = nameof(PlayersTableViewController);

        private readonly ISleeperClient _sleeperClient;
        private readonly IUserDefaults _userDefaults;

        private List<SleeperPlayer> _players = new List<SleeperPlayer>();

        public PlayersTableViewController(ISleeperClient sleeperClient, IUserDefaults userDefaults)
        {
            _sleeperClient = sleeperClient;
            _userDefaults = userDefaults;
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);

            await using var context = new DatabaseContext();

            var stopwatch = new Stopwatch();

            var nextUpdateTime = _userDefaults.SleeperLastUpdated + TimeSpan.FromDays(1);
            Console.WriteLine($"Sleeper last update time: {_userDefaults.SleeperLastUpdated}");
            Console.WriteLine($"Sleeper next update time: {nextUpdateTime}");

            if (nextUpdateTime <= DateTime.Now)
            {
                Console.WriteLine("Updating players");
                stopwatch.Start();
                var sleeperPlayers = await _sleeperClient.GetPlayersAsync();
                stopwatch.Stop();
                Console.WriteLine($"Received sleeper players in {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                foreach (var sleeperPlayer in sleeperPlayers.Values)
                {
                    var dbPlayer = await context.SleeperPlayers.FindAsync(sleeperPlayer.PlayerId);

                    if (dbPlayer == null)
                    {
                        dbPlayer = new SleeperPlayer()
                        {
                            Active = sleeperPlayer.Active,
                            FirstName = sleeperPlayer.FirstName,
                            LastName = sleeperPlayer.LastName,
                            FullName = $"{sleeperPlayer.FirstName} {sleeperPlayer.LastName}",
                            Id = sleeperPlayer.PlayerId,
                            Position = sleeperPlayer.Position,
                            Status = sleeperPlayer.Status,
                            Team = sleeperPlayer.Team
                        };

                        context.SleeperPlayers.Add(dbPlayer);
                    }
                    else
                    {
                        dbPlayer.Active = sleeperPlayer.Active;
                        dbPlayer.FirstName = sleeperPlayer.FirstName;
                        dbPlayer.LastName = sleeperPlayer.LastName;
                        dbPlayer.FullName = $"{sleeperPlayer.FirstName} {sleeperPlayer.LastName}";
                        dbPlayer.Position = sleeperPlayer.Position;
                        dbPlayer.Status = sleeperPlayer.Status;
                        dbPlayer.Team = sleeperPlayer.Team;
                    }
                }

                await context.SaveChangesAsync();

                stopwatch.Stop();
                Console.WriteLine($"Updated sleeper players in {stopwatch.ElapsedMilliseconds} ms");

                _userDefaults.SleeperLastUpdated = DateTime.Now;
            }
            else if (!await context.SleeperPlayers.AnyAsync())
            {
                Console.WriteLine("No players found. Loading players.");
                stopwatch.Restart();
                var sleeperPlayers = await _sleeperClient.GetPlayersAsync();
                stopwatch.Stop();
                Console.WriteLine($"Received sleeper players in {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();
                foreach (var sleeperPlayer in sleeperPlayers.Values)
                {
                    var dbPlayer = new SleeperPlayer()
                    {
                        Active = sleeperPlayer.Active,
                        FirstName = sleeperPlayer.FirstName,
                        LastName = sleeperPlayer.LastName,
                        FullName = $"{sleeperPlayer.FirstName} {sleeperPlayer.LastName}",
                        Id = sleeperPlayer.PlayerId,
                        Position = sleeperPlayer.Position,
                        Status = sleeperPlayer.Status,
                        Team = sleeperPlayer.Team
                    };

                    context.SleeperPlayers.Add(dbPlayer);
                }

                await context.SaveChangesAsync();
                stopwatch.Stop();
                Console.WriteLine($"Saved sleeper players in {stopwatch.ElapsedMilliseconds} ms");
            }

            stopwatch.Restart();
            _players = await context.SleeperPlayers
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
            stopwatch.Stop();
            Console.WriteLine($"Queried players in {stopwatch.ElapsedMilliseconds} ms");

            TableView.ReloadData();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellId, indexPath);

            cell.TextLabel.Text = _players[indexPath.Row].FullName;

            return cell;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _players.Count;
        }
    }
}

