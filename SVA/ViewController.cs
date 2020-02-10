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

        void UpdateTable()
        {
            if (TableDataSource == null)
                return;

            TableDataSource.TableItems = DataStore.Match(name: NameEntry.Text.Trim(), street: StreetEntry.Text.Trim());

            MainTableView.ReloadData();
        }


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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NameEntry.BecomeFirstResponder();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        partial void ResetButton_TouchUpInside(UIButton sender)
        {
            NameEntry.Text = StreetEntry.Text = @"";

            NameEntry.BecomeFirstResponder();

            UpdateTable();
        }

        [Action("searchBar:textDidChange:")]
        public void SearchBar_TextDidChange(UISearchBar sender, string text)
        {
            UpdateTable();
        }
    }
}