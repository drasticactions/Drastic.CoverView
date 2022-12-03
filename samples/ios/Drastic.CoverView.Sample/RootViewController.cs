// <copyright file="RootViewController.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Drawing;
using CoreGraphics;
using Drastic.CoverView;
using Foundation;
using UIKit;

namespace Drastic.CoverView.Sample
{
    public partial class RootViewController : UITableViewController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootViewController"/> class.
        /// </summary>
        public RootViewController()
        {
            this.Title = "Drastic Coverview Demos";
        }

        /// <inheritdoc/>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 3;
        }

        /// <inheritdoc/>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = this.TableView.DequeueReusableCell("Cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "Cell");
            }

            var label = cell.TextLabel;
            label.Font = UIFont.SystemFontOfSize(15);
            switch (indexPath.Row)
            {
                case 0:
                    label.Text = "UIScrollView Demo";
                    break;
                case 1:
                    label.Text = "UITableView Demo";
                    break;
                case 2:
                    label.Text = "UITableView with Top offset Demo";
                    break;
                default:
                    break;
            }

            return cell;
        }

        /// <inheritdoc/>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            this.TableView.DeselectRow(indexPath, true);
            switch (indexPath.Row)
            {
                case 0:
                    this.NavigationController!.PushViewController(new ScrollViewController(), true);
                    break;
                case 1:
                    this.NavigationController!.PushViewController(new TableViewController(), true);
                    break;
                case 2:
                    var label = new UILabel()
                    {
                        Frame = new CGRect(0, 0, 320, 100),
                        BackgroundColor = UIColor.Clear,
                        Lines = 0,
                        Font = UIFont.BoldSystemFontOfSize(20),
                        Text = "This is a header view, This is a header view, This is a header view, This is a header view.",
                    };
                    this.NavigationController!.PushViewController(new TableViewController(label), true);
                    break;
                default:
                    break;
            }
        }
    }
}