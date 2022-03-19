using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Keeper.Core.Database;
using Keeper.Core.Database.Models;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersTableViewController : UITableViewController, IUISearchResultsUpdating
    {
        private const string CellId = nameof(PlayersTableViewController);

        private readonly ISleeperCache _sleeperCache;
        private readonly IUserDefaults _userDefaults;

        private List<SleeperPlayer> _allPlayers = new List<SleeperPlayer>();
        private Dictionary<char, List<SleeperPlayer>> _players = new Dictionary<char, List<SleeperPlayer>>();
        private List<char> _sections = new List<char>();

        private string _filter = string.Empty;

        public PlayersTableViewController(ISleeperCache sleeperCache, IUserDefaults userDefaults)
        {
            _sleeperCache = sleeperCache;
            _userDefaults = userDefaults;
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);
            RefreshControl = new UIRefreshControl()
            {
                AttributedTitle = new NSAttributedString($"Last updated: {_userDefaults.SleeperLastUpdated:g}")
            };
            RefreshControl.ValueChanged += (sender, e) =>
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

            NavigationItem.SearchController = new UISearchController()
            {
                SearchResultsUpdater = this,
                ObscuresBackgroundDuringPresentation = false,
                SearchBar = { Placeholder = "Search for a player" }
            };

            await using var context = new DatabaseContext();
            
            var nextUpdateTime = _userDefaults.SleeperLastUpdated + TimeSpan.FromDays(1);

            if (nextUpdateTime <= DateTime.Now)
            {
                await _sleeperCache.RefreshPlayersAsync();
                _userDefaults.SleeperLastUpdated = DateTime.Now;
            }

            _allPlayers = await _sleeperCache.GetPlayersAsync();
            UpdatePlayersAndSections();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = TableView.DequeueReusableCell(CellId, indexPath);

            var section = _sections[indexPath.Section];
            var player = _players[section][indexPath.Row];
            cell.TextLabel.Text = player.FullName;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = _sections[indexPath.Section];
            var player = _players[section][indexPath.Row];
            Console.WriteLine($"Player: {player.FullName}, Position: {player.Position}, Team: {player.Team}");
            TableView.DeselectRow(indexPath, true);
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

        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            _filter = searchController.SearchBar.Text;
            UpdatePlayersAndSections();
        }

        private void UpdatePlayersAndSections()
        {
            _players = _allPlayers
                .Where(x => x.FullName.Contains(_filter, StringComparison.CurrentCultureIgnoreCase))
                .GroupBy(x =>
                {
                    char firstLetter = char.ToUpper(x.LastName.First());
                    return char.IsDigit(firstLetter) ? '#' : firstLetter;
                })
                .ToDictionary(
                    x => x.Key,
                    x => x.OrderBy(y => y.LastName).ThenBy(y => y.FirstName).ToList());
            _sections = _players.Keys.OrderBy(x => x).ToList();
            TableView.ReloadData();
        }
    }
}

