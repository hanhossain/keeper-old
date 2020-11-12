using UIKit;

namespace Keeper.iOS.Extensions
{
    public static class UITableViewExtensions
    {
        public static void RegisterClassForCellReuse<T>(this UITableView tableView, string cellId)
            where T: UITableViewCell
        {
            tableView.RegisterClassForCellReuse(typeof(T), cellId);
        }
    }
}
