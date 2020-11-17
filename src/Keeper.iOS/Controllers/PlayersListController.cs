using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Keeper.Core.Delegates;
using Keeper.Core.Models;
using Keeper.Core.Services;
using Keeper.iOS.Extensions;
using Keeper.iOS.Views;
using UIKit;

namespace Keeper.iOS.Controllers
{
    public class PlayersListController : UITableViewController, IUISearchResultsUpdating, IProgressDelegate
    {
        private const string CellId = "playerCell";

        private readonly PlayerService _playerService = new PlayerService();
        private readonly UISearchController _searchController = new UISearchController(searchResultsController: null)
        {
            ObscuresBackgroundDuringPresentation = false,
            SearchBar = { Placeholder = "Search Players" }
        };

        private Dictionary<char, List<Player>> _players = new Dictionary<char, List<Player>>();
        private Dictionary<char, List<Player>> _filteredPlayers = new Dictionary<char, List<Player>>();
        private List<char> _sections = new List<char>();
        private List<char> _filteredSections = new List<char>();
        private UIProgressView _progressView;
        private UIAlertController _alertController;

        public bool IsFiltering => _searchController.Active && !string.IsNullOrWhiteSpace(_searchController.SearchBar.Text);

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            Title = "Players";
            TableView.RegisterClassForCellReuse<SubtitleTableViewCell>(CellId);

            // setup search controller
            _searchController.SearchResultsUpdater = this;
            NavigationItem.SearchController = _searchController;
            DefinesPresentationContext = true;

            var players = await _playerService.GetPlayersAsync(this);
            (_players, _sections) = GetPlayersAndSections(players);

            TableView.ReloadData();
        }

        #region Table View Data Source

        public override nint NumberOfSections(UITableView tableView)
        {
            var sections = IsFiltering ? _filteredSections : _sections;
            return sections.Count();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            var (players, sections) = IsFiltering ? (_filteredPlayers, _filteredSections) : (_players, _sections);
            return players[sections[(int)section]].Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            var sections = IsFiltering ? _filteredSections : _sections;
            return sections[(int)section].ToString();
        }

        public override string[] SectionIndexTitles(UITableView tableView)
        {
            var sections = IsFiltering ? _filteredSections : _sections;
            return sections.Select(x => x.ToString()).ToArray();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId, indexPath);
            var (players, sections) = IsFiltering ? (_filteredPlayers, _filteredSections) : (_players, _sections);

            var section = sections[indexPath.Section];
            var player = players[section][indexPath.Row];

            cell.TextLabel.Text = player.Name;
            cell.DetailTextLabel.Text = player.PositionAndTeam;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var (players, sections) = IsFiltering ? (_filteredPlayers, _filteredSections) : (_players, _sections);
            var section = sections[indexPath.Section];
            var player = players[section][indexPath.Row];
            Console.WriteLine(player.Name);
            tableView.DeselectRow(indexPath, true);
        }

        #endregion

        #region IUISearchResultsUpdating

        public async void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            var query = searchController.SearchBar.Text;
            await FilterPlayersAsync(query);
            TableView.ReloadData();
        }

        #endregion

        #region IProgressDelegate

        public void ShowProgressIndicator()
        {
            InvokeOnMainThread(() =>
            {
                _alertController = UIAlertController.Create("Loading...", null, UIAlertControllerStyle.Alert);
                PresentViewController(_alertController, true, () =>
                {
                    var margin = 8.0;
                    var rect = new CGRect(margin, _alertController.View.Frame.Height + margin, _alertController.View.Frame.Width - margin * 2.0, 2.0);

                    _progressView = new UIProgressView(rect)
                    {
                        Progress = 0.5f,
                        TintColor = View.TintColor
                    };

                    _alertController.View.AddSubview(_progressView);
                });
            });
        }

        public void DismissProgressIndicator()
        {
            InvokeOnMainThread(() => _alertController.DismissViewController(true, null));
        }

        #endregion

        #region Private Methods

        private (Dictionary<char, List<Player>>, List<char>) GetPlayersAndSections(IEnumerable<Player> originalPlayers)
        {
            var players = new Dictionary<char, List<Player>>();
            var sections = new List<char>();

            foreach (var player in originalPlayers)
            {
                var firstLetter = char.ToUpper(player.Name.First());
                if (firstLetter >= '0' && firstLetter <= '9')
                {
                    firstLetter = '#';
                }

                if (sections.LastOrDefault() != firstLetter)
                {
                    sections.Add(firstLetter);
                }

                if (!players.TryGetValue(firstLetter, out var playersInSection))
                {
                    playersInSection = new List<Player>();
                    players[firstLetter] = playersInSection;
                }

                playersInSection.Add(player);
            }

            return (players, sections);
        }

        private async Task FilterPlayersAsync(string query)
        {
            var players = await _playerService.GetPlayersAsync(this);
            var filtered = players.Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

            (_filteredPlayers, _filteredSections) = GetPlayersAndSections(filtered);
        }

        #endregion
    }
}
