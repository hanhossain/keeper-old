using System;
using System.Collections.Generic;
using Foundation;
using Keeper.iOS.Extensions;
using Keeper.iOS.Views;
using MathNet.Numerics.Statistics;
using UIKit;

namespace Keeper.iOS.ViewControllers;

public class StatisticSummaryViewController : UITableViewController
{
    private const string CellId = nameof(StatisticSummaryViewController);
    private readonly List<(string, double)> _statistics;

    public StatisticSummaryViewController(IEnumerable<double> rawData)
    {
        var descriptiveStatistics = new DescriptiveStatistics(rawData);
        var fiveNumberSummary = rawData.FiveNumberSummary();

        _statistics = new List<(string, double)>
        {
            ("Mean", descriptiveStatistics.Mean),
            ("Count", descriptiveStatistics.Count),
            ("Minimum", descriptiveStatistics.Minimum),
            ("Lower Quartile", fiveNumberSummary[1]),
            ("Median", fiveNumberSummary[2]),
            ("Upper Quartile", fiveNumberSummary[3]),
            ("Maximum", descriptiveStatistics.Maximum),
            ("Standard Deviation", descriptiveStatistics.StandardDeviation),
            ("Variance", descriptiveStatistics.Variance),
            ("Skewness", descriptiveStatistics.Skewness),
            ("Kurtosis", descriptiveStatistics.Kurtosis)
        };

        Title = "Sumary";
        TabBarItem.Image = UIImage.GetSystemImage("sum");
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        View.BackgroundColor = UIColor.SystemBackground;
        TableView.RegisterClassForCellReuse<RightDetailTableViewCell>(CellId);
    }

    public override nint RowsInSection(UITableView tableView, nint section)
    {
        return _statistics.Count;
    }

    public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
    {
        var cell = tableView.DequeueReusableCell<RightDetailTableViewCell>(CellId, indexPath);

        var (measure, value) = _statistics[indexPath.Row];

        cell.TextLabel.Text = measure;
        cell.DetailTextLabel.Text = value.ToString();

        return cell;
    }
}
