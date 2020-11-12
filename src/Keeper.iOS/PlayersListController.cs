using System;
using Keeper.iOS.Extensions;
using Foundation;
using UIKit;

namespace Keeper.iOS
{
    public class PlayersListController : UITableViewController
    {
        private const string CellId = "playerCell";

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.RegisterClassForCellReuse<UITableViewCell>(CellId);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId, indexPath);

            cell.TextLabel.Text = "Hello world";

            return cell;
        }
    }
}
