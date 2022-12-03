// <copyright file="ScrollViewController.cs" company="Drastic Actions">
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
    public partial class ScrollViewController : UIViewController
    {
        private UIScrollView scrollView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewController"/> class.
        /// </summary>
        public ScrollViewController()
        {
            this.Title = "UIScrollView Demo";

            this.EdgesForExtendedLayout = UIRectEdge.None;
            this.AutomaticallyAdjustsScrollViewInsets = false;
        }

        /// <inheritdoc/>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            nfloat coverViewHeight = 200;

            this.scrollView = new UIScrollView(this.View!.Bounds)
            {
                ContentSize = new CGSize(this.View.Bounds.Size.Width, 600),
            };

            this.scrollView.AddCoverImage(UIImage.FromBundle("joey.png")!, coverViewHeight);
            this.View.AddSubview(this.scrollView);

            var label = new UILabel(new CGRect(20, coverViewHeight, this.View.Bounds.Size.Width - 40, 600 - coverViewHeight))
            {
                Lines = 0,
                Font = UIFont.SystemFontOfSize(22),
                Text = "Drastic CoverView is a parallax top view with real time blur effect to any UIScrollView, inspired by Twitter for iOS.\n\nCompletely created using UIKit framework.\n\nEasy to drop into your project.\n\nYou can add this feature to your own project, Drastic.CoverView is easy-to-use.",
            };

            this.scrollView.AddSubview(label);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            this.scrollView.RemoveCoverImage();

            base.Dispose(disposing);
        }
    }
}