using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using Keeper.Core.Models;
using Keeper.Core.Services;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersTableViewController : UITableViewController
    {
        private const string CellId = nameof(PlayersTableViewController);

        private readonly IPlayerService _playerService;

        private List<Player> _players;

        public PlayersTableViewController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            Console.WriteLine($"Currently in {nameof(PlayersTableViewController)}");

            TableView.RegisterClassForCellReuse<SubtitleTableViewCell>(CellId);

            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Getting players...");
            var players = await _playerService.GetPlayersAsync();
            stopwatch.Stop();
            Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds}ms to get players");

            _players = players;

            InvokeOnMainThread(() => TableView.ReloadData());
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _players?.Count ?? 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell<SubtitleTableViewCell>(CellId, indexPath);

            var player = _players?[indexPath.Row];

            if (player == null)
            {
                cell.TextLabel.Text = "Loading...";
            }
            else
            {
                cell.TextLabel.Text = player.Name;
                cell.DetailTextLabel.Text = $"{player.Position} - {player.Team}";
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var player = _players[indexPath.Row];
            Console.WriteLine("Selected");
        }
    }
}
