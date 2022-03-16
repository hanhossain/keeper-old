using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation;
using Keeper.Core;
using Keeper.Core.Database;
using Keeper.Core.Sleeper;
using Microsoft.EntityFrameworkCore;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersTableViewController : UITableViewController
    {
        private const string CellId = nameof(PlayersTableViewController);

        private readonly ISleeperCache _sleeperCache;
        private readonly IUserDefaults _userDefaults;

        private List<SleeperPlayer> _players = new List<SleeperPlayer>();

        public PlayersTableViewController(ISleeperCache sleeperCache, IUserDefaults userDefaults)
        {
            _sleeperCache = sleeperCache;
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
                await _sleeperCache.RefreshPlayersAsync();
                _userDefaults.SleeperLastUpdated = DateTime.Now;
            }

            _players = await _sleeperCache.GetPlayersAsync();
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

