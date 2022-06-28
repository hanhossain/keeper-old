using Foundation;
using UIKit;

namespace Keeper.iOS;

public class RightDetailTableViewCell : UITableViewCell
{
    [Export("initWithStyle:reuseIdentifier:")]
    public RightDetailTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(UITableViewCellStyle.Value1, reuseIdentifier)
    {
    }
}
