using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Keeper.Core.Database;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersTableViewController : UITableViewController
    {
        private const string CellId = nameof(PlayersTableViewController);

        private readonly ISleeperCache _sleeperCache;
        private readonly IUserDefaults _userDefaults;

        private Dictionary<char, List<SleeperPlayer>> _players = new Dictionary<char, List<SleeperPlayer>>();
        private List<char> _sections = new List<char>();

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
            RefreshControl = new UIRefreshControl()
            {
                AttributedTitle = new NSAttributedString($"Last updated: {_userDefaults.SleeperLastUpdated:g}")
            };
            RefreshControl.ValueChanged += (object sender, EventArgs e) =>
            {
                _ = Task.Run(async () =>
                {
                    await _sleeperCache.RefreshPlayersAsync();
                    _userDefaults.SleeperLastUpdated = DateTime.Now;
                    InvokeOnMainThread(() =>
                    {
                        RefreshControl.AttributedTitle = new NSAttributedString($"Last updated: {_userDefaults.SleeperLastUpdated:g}");
                        RefreshControl.EndRefreshing();
                    });
                });
            };

            await using var context = new DatabaseContext();

            var stopwatch = new Stopwatch();

            var nextUpdateTime = _userDefaults.SleeperLastUpdated + TimeSpan.FromDays(1);

            if (nextUpdateTime <= DateTime.Now)
            {
                await _sleeperCache.RefreshPlayersAsync();
                _userDefaults.SleeperLastUpdated = DateTime.Now;
            }

            var players = await _sleeperCache.GetPlayersAsync();
            _players = players
                .GroupBy(x =>
                {
                    char firstLetter = char.ToUpper(x.LastName.First());
                    return char.IsDigit(firstLetter) ? '#' : firstLetter;
                })
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList());
            _sections = _players.Keys.OrderBy(x => x).ToList();

            TableView.ReloadData();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellId, indexPath);

            var section = _sections[indexPath.Section];
            var player = _players[section][indexPath.Row];
            cell.TextLabel.Text = player.FullName;

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return _sections.Count;
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            var sectionId = _sections[(int)section];
            return _players[sectionId].Count;
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            return _sections
                .Select(x => x.ToString())
                .ToArray();
        }
    }
}

