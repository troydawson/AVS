using Foundation;
using System;
using UIKit;

using static Logger;

namespace SVA
{
    public class DataSource : UITableViewSource
    {
        string[] TableItems;
        string CellIdentifier = "TableCell";

        public DataSource(string[] items)
        {
            TableItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier)
                ?? new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

            cell.TextLabel.Text = TableItems[indexPath.Row];

            return cell;
        }
    }

    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            MainTableView.Source = new DataSource(DataStore.Match(name: "", street: ""));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void ResetButton_TouchUpInside(UIButton sender)
        {
            msg("[i] -- Reset!");
        }
    }
}