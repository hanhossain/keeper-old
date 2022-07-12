using UIKit;

namespace Keeper.iOS.Extensions;

public static class UIViewExtensions
{
    public static void PinTo(this UIView source, UILayoutGuide target)
    {
        source.LeadingAnchor.ConstraintEqualTo(target.LeadingAnchor).Active = true;
        source.TopAnchor.ConstraintEqualTo(target.TopAnchor).Active = true;
        source.TrailingAnchor.ConstraintEqualTo(target.TrailingAnchor).Active = true;
        source.BottomAnchor.ConstraintEqualTo(target.BottomAnchor).Active = true;
    }
}
