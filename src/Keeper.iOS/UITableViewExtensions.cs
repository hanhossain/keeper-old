using System;
using UIKit;

namespace Keeper.iOS
{
    public static class UITableViewExtensions
    {
        public static void RegisterClassForCellReuse<T>(this UITableView tableView, string reuseIdentifier)
            where T : UITableViewCell
        {
            tableView.RegisterClassForCellReuse(typeof(T), reuseIdentifier);
        }
    }
}

