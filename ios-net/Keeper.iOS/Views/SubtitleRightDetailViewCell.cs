using Foundation;
using UIKit;

namespace Keeper.iOS.Views;

public class SubtitleRightDetailViewCell : UITableViewCell
{
    [Export("initWithStyle:reuseIdentifier:")]
    public SubtitleRightDetailViewCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(UITableViewCellStyle.Default, reuseIdentifier)
    {
        var leftStackView = new UIStackView(new[] { MainLabel, SubtitleLabel })
        {
            Axis = UILayoutConstraintAxis.Vertical,
            Spacing = 8
        };

        var mainStackView = new UIStackView(new UIView[] { leftStackView, RightLabel })
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Distribution = UIStackViewDistribution.EqualSpacing,
            LayoutMargins = new UIEdgeInsets(8, 8, 8, 8),
            LayoutMarginsRelativeArrangement = true,
            Spacing = 4
        };
        ContentView.AddSubview(mainStackView);

        mainStackView.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor).Active = true;
        mainStackView.LeftAnchor.ConstraintEqualTo(ContentView.LeftAnchor).Active = true;
        mainStackView.RightAnchor.ConstraintEqualTo(ContentView.RightAnchor).Active = true;
        mainStackView.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor).Active = true;
    }

    public UILabel MainLabel { get; } = new UILabel()
    {
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    public UILabel SubtitleLabel { get; } = new UILabel()
    {
        TranslatesAutoresizingMaskIntoConstraints = false,
        Font = UIFont.PreferredCaption1
    };

    public UILabel RightLabel { get; } = new UILabel()
    {
        TranslatesAutoresizingMaskIntoConstraints = false
    };
}
