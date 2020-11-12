using System;
using Keeper.iOS.Extensions;
using Foundation;
using UIKit;
using System.Collections.Generic;
using Keeper.Core.Models;
using Keeper.Core.Services;

namespace Keeper.iOS
{
    public class PlayersListController : UITableViewController
    {
        private const string CellId = "playerCell";

        private readonly PlayerService _playerService = new PlayerService();

        private List<Player> _players = new List<Player>();

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);

            _players = await _playerService.GetPlayersAsync();

            TableView.ReloadData();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _players.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId, indexPath);

            var player = _players[indexPath.Row];
            cell.TextLabel.Text = player.Name;

            return cell;
        }
    }
}
