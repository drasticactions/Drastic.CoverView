// <copyright file="TableViewController.cs" company="Drastic Actions">
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
    public partial class TableViewController : UITableViewController
    {
        private UIView topView;

        public TableViewController()
        {
            this.Title = "UITableView Demo";

            this.EdgesForExtendedLayout = UIRectEdge.All;
            this.Init();
        }

        public TableViewController(UIView topView)
        {
            this.topView = topView;
            this.Title = "UITableView Demo";
            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.Init();
        }

        /// <inheritdoc/>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.TableView.ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
        }

        /// <inheritdoc/>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 20;
        }

        /// <inheritdoc/>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell? cell = this.TableView.DequeueReusableCell("Cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "Cell");
            }
            cell.TextLabel.Text = "Cell " + (indexPath.Row + 1);
            return cell;
        }

        private new void Init()
        {
            nfloat coverViewHeight = 200;

            this.TableView.AddCoverImage(UIImage.FromBundle("joey.png")!, this.topView, coverViewHeight);
            nfloat topViewHeight = 0;
            if (this.topView != null)
            {
                topViewHeight = this.topView.Bounds.Size.Height;
            }

            this.TableView.TableHeaderView = new UIView(new CGRect(0, 0, 320, coverViewHeight + topViewHeight));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}