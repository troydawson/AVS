using Foundation;
using System;
using UIKit;

using static Logger;

namespace SVA
{
    public class DataSource : UITableViewSource
    {
        public string[] TableItems { get; set; }
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

    public partial class ViewController : UIViewController, IUISearchBarDelegate
    {
        DataSource? TableDataSource { get; set; }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NameEntry.WeakDelegate = this;
            StreetEntry.WeakDelegate = this;


            MainTableView.Source = TableDataSource = new DataSource(DataStore.Match(name: "", street: ""));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void ResetButton_TouchUpInside(UIButton sender)
        {
            msg("[i] -- Reset!");
        }

        [Action("searchBar:textDidChange:")]
        public void SearchBar_TextDidCHange(UISearchBar sender, string text)
        {
            if (TableDataSource == null)
                return;

            TableDataSource.TableItems = DataStore.Match(name: NameEntry.Text.Trim(), street: StreetEntry.Text.Trim());

            MainTableView.ReloadData();
        }
    }
}