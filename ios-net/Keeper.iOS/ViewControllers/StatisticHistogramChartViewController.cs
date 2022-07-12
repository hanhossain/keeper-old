using UIKit;

namespace Keeper.iOS.ViewControllers;

public class StatisticHistogramChartViewController : UIViewController
{
    public StatisticHistogramChartViewController()
    {
        Title = "Histogram";
        TabBarItem.Image = UIImage.GetSystemImage("chart.bar.xaxis");
        View.BackgroundColor = UIColor.SystemBackground;
    }
}
