using Foundation;
using System;
using UIKit;

namespace Keeper.iOS
{
    public class ViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            View.BackgroundColor = UIColor.SystemBlueColor;
            Title = "Players";
        }
    }
}