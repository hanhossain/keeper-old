using System.Collections.Generic;
using System.Linq;
using Foundation;
using Keeper.iOS.Extensions;
using Syncfusion.SfChart.iOS;
using UIKit;

namespace Keeper.iOS.ViewControllers;

public class StatisticTimeChartViewController : UIViewController
{
    private readonly List<TimeChartData> _aggregatedStatistics;

    public StatisticTimeChartViewController(Dictionary<int, double> aggregatedStatistics)
    {
        _aggregatedStatistics = aggregatedStatistics
            .OrderBy(x => x.Key)
            .Select(x => new TimeChartData
            {
                Week = x.Key,
                Value = x.Value
            })
            .ToList();

        Title = "Time chart";
        TabBarItem.Image = UIImage.GetSystemImage("chart.xyaxis.line");
        View.BackgroundColor = UIColor.SystemBackground;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        var chart = new SFChart()
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            PrimaryAxis = new SFNumericalAxis()
            {
                Title =
                {
                    Text = "Week".ToNSString(),
                    Color = UIColor.Label
                },
                Interval = new NSNumber(1)
            },
            SecondaryAxis = new SFNumericalAxis()
        };

        var series = new SFLineSeries()
        {
            ItemsSource = _aggregatedStatistics,
            XBindingPath = nameof(TimeChartData.Week),
            YBindingPath = nameof(TimeChartData.Value),
            Color = UIColor.SystemBlue,
            DataMarker =
            {
                ShowLabel = true,
                ShowMarker = true
            }
        };

        chart.Series.Add(series);

        View.AddSubview(chart);
        chart.PinTo(View.SafeAreaLayoutGuide);
    }

    public class TimeChartData
    {
        public int Week { get; set; }

        public double Value { get; set; }
    }
}
