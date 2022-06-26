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
    private List<SleeperPlayer> _players = new List<SleeperPlayer>();

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
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList();

        TableView.ReloadData();
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        return _players.Count;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell<SubtitleTableViewCell>(CellId, indexPath);

        var player = _players[indexPath.Row];
        cell.TextLabel.Text = $"{player.FirstName} {player.LastName}";
        cell.DetailTextLabel.Text = $"{player.Position} - {player.Team}";

        return cell;
    }
}
