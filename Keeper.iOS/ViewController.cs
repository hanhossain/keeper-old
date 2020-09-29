using System;
using UIKit;

namespace Keeper.iOS
{
    public class ViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.SystemBackgroundColor;

            Console.WriteLine("Hello world from view controller");
        }
    }
}
