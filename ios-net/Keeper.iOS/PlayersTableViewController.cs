using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Foundation;
using Keeper.iOS.Extensions;
using Keeper.iOS.Sleeper;
using Keeper.iOS.Sleeper.Models;
using UIKit;

namespace Keeper.iOS;

public class PlayersTableViewController : UITableViewController
{
    private const string CellId = nameof(PlayersTableViewController);

    private readonly SleeperClient _sleeperClient;

    private List<char> _sectionHeaders = new List<char>();
    private Dictionary<char, List<SleeperPlayer>> _players = new Dictionary<char, List<SleeperPlayer>>();

    public PlayersTableViewController(HttpClient httpClient)
    {
        _sleeperClient = new SleeperClient(httpClient);
    }

    public override async void ViewDidLoad()
    {
        base.ViewDidLoad();
        TableView.RegisterClassForCellReuse<SubtitleTableViewCell>(CellId);

        Title = "Players";

        var validPositions = new HashSet<string>() { "QB", "RB", "WR", "TE", "K", "DEF" };
        var players = await _sleeperClient.GetPlayersAsync();
        _players = players
            .Values
            .Where(x => x.Active && validPositions.Contains(x.Position))
            .GroupBy(x =>
            {
                var firstLetter = x.LastName.First();
                return char.IsNumber(firstLetter) ? '#' : firstLetter;
            })
            .ToDictionary(x => x.Key, x => x.ToList());

        _sectionHeaders = _players.Keys.OrderBy(x => x).ToList();

        TableView.ReloadData();
    }

    public override nint NumberOfSections(UITableView tableView)
    {
        return _sectionHeaders.Count;
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        var sectionHeader = _sectionHeaders[(int)section];
        return _players[sectionHeader].Count;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell<SubtitleTableViewCell>(CellId, indexPath);

        var sectionHeader = _sectionHeaders[indexPath.Section];
        var player = _players[sectionHeader][indexPath.Row];

        cell.TextLabel.Text = $"{player.FirstName} {player.LastName}";
        cell.DetailTextLabel.Text = $"{player.Position} - {player.Team}";

        return cell;
    }

    public override string[] SectionIndexTitles(UITableView tableView)
    {
        return _sectionHeaders.Select(x => x.ToString()).ToArray();
    }
}
