using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Foundation;
using Keeper.iOS.Extensions;
using Keeper.iOS.Sleeper;
using Keeper.iOS.Sleeper.Models;
using UIKit;

namespace Keeper.iOS;

public class PlayersTableViewController : UITableViewController, IUISearchResultsUpdating
{
    private const string CellId = nameof(PlayersTableViewController);

    private readonly SleeperClient _sleeperClient;

    private List<char> _sectionHeaders = new List<char>();
    private Dictionary<char, List<SleeperPlayer>> _players = new Dictionary<char, List<SleeperPlayer>>();
    private Dictionary<char, List<SleeperPlayer>> _filteredPlayers = new Dictionary<char, List<SleeperPlayer>>();
    private Dictionary<string, SleeperSeasonStatistics> _seasonStatistics = new Dictionary<string, SleeperSeasonStatistics>();

    public PlayersTableViewController(HttpClient httpClient)
    {
        _sleeperClient = new SleeperClient(httpClient);
    }

    public override async void ViewDidLoad()
    {
        base.ViewDidLoad();
        TableView.RegisterClassForCellReuse<SubtitleRightDetailViewCell>(CellId);
        Title = "Players";

        var searchController = new UISearchController()
        {
            SearchResultsUpdater = this
        };
        NavigationItem.SearchController = searchController;

        var playersTask = LoadPlayersAsync();
        var statisticsTask = LoadSeasonStatisticsAsync();
        await Task.WhenAll(playersTask, statisticsTask);

        TableView.ReloadData();
    }

    public override nint NumberOfSections(UITableView tableView)
    {
        return _sectionHeaders.Count;
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        var sectionHeader = _sectionHeaders[(int)section];
        return _filteredPlayers[sectionHeader].Count;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell<SubtitleRightDetailViewCell>(CellId, indexPath);

        var sectionHeader = _sectionHeaders[indexPath.Section];
        var player = _filteredPlayers[sectionHeader][indexPath.Row];

        cell.MainLabel.Text = $"{player.FirstName} {player.LastName}";

        if (_seasonStatistics[player.PlayerId].Stats?.TryGetValue("pts_std", out var points) == true)
        {
            cell.RightLabel.Text = points.ToString();
        }

        cell.SubtitleLabel.Text = $"{player.Position} - {player.Team}";

        return cell;
    }

    public override string[] SectionIndexTitles(UITableView tableView)
    {
        return _sectionHeaders.Select(x => x.ToString()).ToArray();
    }

    private async Task LoadPlayersAsync()
    {
        var validPositions = new HashSet<string>() { "QB", "RB", "WR", "TE", "K", "DEF" };
        var players = await _sleeperClient.GetPlayersAsync();

        var playerQuery = players
            .Values
            .Where(x => x.Active && validPositions.Contains(x.Position));
        _players = ProcessPlayers(playerQuery);

        FilterPlayers(_players);
    }

    private Dictionary<char, List<SleeperPlayer>> ProcessPlayers(IEnumerable<SleeperPlayer> players)
    {
        return players
            .GroupBy(x =>
            {
                var firstLetter = x.LastName.First();
                return char.IsNumber(firstLetter) ? '#' : firstLetter;
            })
            .ToDictionary(
                x => x.Key,
                x => x.OrderBy(y => y.LastName)
                    .ThenBy(y => y.FirstName)
                    .ToList());
    }

    private void FilterPlayers(Dictionary<char, List<SleeperPlayer>> players)
    {
        _filteredPlayers = players;
        _sectionHeaders = players.Keys.OrderBy(x => x).ToList();
    }

    private async Task LoadSeasonStatisticsAsync()
    {
        var statistics = await _sleeperClient.GetSeasonStatisticsAsync();
        _seasonStatistics = statistics.ToDictionary(x => x.PlayerId);
    }

    public void UpdateSearchResultsForSearchController(UISearchController searchController)
    {
        var query = searchController.SearchBar.Text;

        if (string.IsNullOrEmpty(query))
        {
            FilterPlayers(_players);
        }
        else
        {
            var playerQuery = _players
                .Values
                .SelectMany(x => x)
                .Where(x => $"{x.FirstName} {x.LastName}".Contains(query, StringComparison.InvariantCultureIgnoreCase));
            var filteredPlayers = ProcessPlayers(playerQuery);
            FilterPlayers(filteredPlayers);
        }

        TableView.ReloadData();
    }
}
