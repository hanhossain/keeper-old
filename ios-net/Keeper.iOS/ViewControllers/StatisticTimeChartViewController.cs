using System.Collections.Generic;
using UIKit;

namespace Keeper.iOS.ViewControllers;

public class StatisticTimeChartViewController : UIViewController
{
    private readonly Dictionary<int, double> _aggregatedStatistics;

    public StatisticTimeChartViewController(Dictionary<int, double> aggregatedStatistics)
    {
        _aggregatedStatistics = aggregatedStatistics;

        Title = "Time chart";
        TabBarItem.Image = UIImage.GetSystemImage("chart.xyaxis.line");
        View.BackgroundColor = UIColor.SystemBackground;
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();
    }
}
