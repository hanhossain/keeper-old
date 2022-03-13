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
        private List<SleeperPlayer> _players = new List<SleeperPlayer>();

        public PlayersTableViewController(ISleeperClient sleeperClient)
        {
            _sleeperClient = sleeperClient;
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);

            await using var context = new DatabaseContext();

            var stopwatch = new Stopwatch();

            if (!await context.SleeperPlayers.AnyAsync())
            {
                stopwatch.Start();
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

            stopwatch.Start();
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

