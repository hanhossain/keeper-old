using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Keeper.Core.Sleeper;
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
            var players = await _sleeperClient.GetPlayersAsync();
            _players = players.Values
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();
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

