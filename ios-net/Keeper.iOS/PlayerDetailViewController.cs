using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Keeper.iOS.Extensions;
using Keeper.iOS.Sleeper;
using Keeper.iOS.Sleeper.Models;
using UIKit;

namespace Keeper.iOS;

public class PlayerDetailViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
{
    private const string CellId = nameof(PlayerDetailViewController);

    private readonly SleeperPlayer _player;
    private readonly SleeperClient _sleeperClient;
    private readonly SleeperSeasonStatistics _seasonStatistics;
    private readonly List<string> _statisticsKeys;

    private Dictionary<int, SleeperPlayerStatistics> _statistics;
    private Dictionary<string, Dictionary<int, double>> _aggregatedStatistics;

    public PlayerDetailViewController(SleeperPlayer player, SleeperClient sleeperClient, SleeperSeasonStatistics seasonStatistics)
    {
        _player = player;
        _sleeperClient = sleeperClient;
        _seasonStatistics = seasonStatistics;
        _statisticsKeys = seasonStatistics.Stats.Keys.OrderBy(x => x).ToList();

        Title = $"{player.FirstName} {player.LastName}";
    }

    public override async void ViewDidLoad()
    {
        base.ViewDidLoad();
        View.BackgroundColor = UIColor.SystemBackground;

        var stackView = new UIStackView()
        {
            Axis = UILayoutConstraintAxis.Vertical,
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        View.AddSubview(stackView);
        stackView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
        stackView.LeftAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
        stackView.RightAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.RightAnchor).Active = true;
        stackView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;

        var metadataStackView = new UIStackView();
        stackView.AddArrangedSubview(metadataStackView);

        _statistics = await _sleeperClient.GetPlayerStatisticsAsync(_player.PlayerId);

        _aggregatedStatistics = _statistics
            .Values
            .Where(x => x != null)
            .SelectMany(
                x => x.Stats.Select(y => new
                {
                    Week = x.Week,
                    Stat = y.Key,
                    Value = y.Value
                }))
            .GroupBy(x => x.Stat)
            .ToDictionary(
                x => x.Key,
                x => x.ToDictionary(y => y.Week, y => y.Value));

        var avatarBytes = await _sleeperClient.GetAvatarAsync(_player.PlayerId);
        var avatarImage = avatarBytes == null
            ? UIImage.GetSystemImage("person.fill.questionmark", UIImageSymbolConfiguration.Create(71))
            : UIImage.LoadFromData(NSData.FromArray(avatarBytes));
        var avatarView = new UIImageView(avatarImage)
        {
            TintColor = UIColor.SystemGray,
            ContentMode = UIViewContentMode.ScaleAspectFit
        };
        metadataStackView.AddArrangedSubview(avatarView);

        var positionTeam = new UILabel()
        {
            Text = $"{_player.Position} - {_player.Team}"
        };
        metadataStackView.AddArrangedSubview(positionTeam);

        var tableView = new UITableView()
        {
            DataSource = this,
            Delegate = this
        };
        stackView.AddArrangedSubview(tableView);

        tableView.RegisterClassForCellReuse<RightDetailTableViewCell>(CellId);
    }

    #region IUITableViewDataSource

    public nint RowsInSection(UITableView tableView, nint section)
    {
        return _seasonStatistics.Stats.Count;
    }

    public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell<RightDetailTableViewCell>(CellId, indexPath);

        var key = _statisticsKeys[indexPath.Row];
        var value = _seasonStatistics.Stats[key];
        cell.TextLabel.Text = key;
        cell.DetailTextLabel.Text = value.ToString();

        if (_aggregatedStatistics.ContainsKey(key))
        {
            cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }
        else
        {
            cell.Accessory = UITableViewCellAccessory.None;
        }

        return cell;
    }

    #endregion

    #region IUITableViewDelegate

    [Export("tableView:didSelectRowAtIndexPath:")]
    public void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        var key = _statisticsKeys[indexPath.Row];

        if (_aggregatedStatistics.TryGetValue(key, out var aggregatedStatistics))
        {
            var statisticViewController = new StatisticDetailViewController(key, aggregatedStatistics.Values);
            NavigationController.PushViewController(statisticViewController, true);
        }

        tableView.DeselectRow(indexPath, true);
    }

    #endregion
}
