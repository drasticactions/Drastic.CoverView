// <copyright file="CoverView.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Accelerate;

namespace Drastic.CoverView
{
    /// <summary>
    /// Cover View.
    /// </summary>
    public class CoverView : UIImageView
    {
        /// <summary>
        /// The default cover view height.
        /// </summary>
        public static readonly nfloat DefaultCoverViewHeight = 200;

        private UIScrollView? scrollView;
        private UIView? topView;
        private List<UIImage> blurredImages = new List<UIImage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverView"/> class.
        /// </summary>
        /// <param name="coverViewHeight">The Cover View Height.</param>
        public CoverView(nfloat? coverViewHeight)
        {
            var cvh = coverViewHeight ?? DefaultCoverViewHeight;
            this.Initialize(null, cvh);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverView"/> class.
        /// </summary>
        /// <param name="topView">Header View.</param>
        /// <param name="coverViewHeight">The Cover View Height.</param>
        public CoverView(UIView? topView, nfloat? coverViewHeight)
        {
            var cvh = coverViewHeight ?? DefaultCoverViewHeight;
            this.Initialize(topView, cvh);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverView"/> class.
        /// </summary>
        /// <param name="frame">Base Frame.</param>
        /// <param name="coverViewHeight">The Cover View Height.</param>
        public CoverView(CGRect frame, nfloat? coverViewHeight)
            : base(frame)
        {
            var cvh = coverViewHeight ?? DefaultCoverViewHeight;
            this.Initialize(null, cvh);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverView"/> class.
        /// </summary>
        /// <param name="frame">Base Frame.</param>
        /// <param name="topView">Header View.</param>
        /// <param name="coverViewHeight">The Cover View Height.</param>
        public CoverView(CGRect frame, UIView topView, nfloat? coverViewHeight)
            : base(frame)
        {
            var cvh = coverViewHeight ?? DefaultCoverViewHeight;
            this.Initialize(topView, cvh);
        }

        /// <summary>
        /// Gets or sets the ScrollView for the CoverView.
        /// </summary>
        public UIScrollView? ScrollView
        {
            get
            {
                return this.scrollView;
            }

            set
            {
                if (this.ScrollView != null)
                {
                    this.ScrollView.RemoveObserver(this, "contentOffset");
                }

                this.scrollView = value;
                this.ScrollView?.AddObserver(this, "contentOffset", NSKeyValueObservingOptions.New, IntPtr.Zero);
            }
        }

        /// <inheritdoc/>
        public override UIImage? Image
        {
            get
            {
                return base.Image;
            }

            set
            {
                base.Image = value;
                this.PrepareForBlurImages();
            }
        }

        /// <summary>
        /// Gets the CoverView height.
        /// </summary>
        public nfloat CoverViewHeight { get; private set; }

        /// <summary>
        /// Gets the width of the CoverView.
        /// </summary>
        public nfloat Width => this.ScrollView?.Frame.Width ?? new NFloat(0);

        /// <inheritdoc/>
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (this.ScrollView?.ContentOffset.Y < 0)
            {
                var offset = -this.ScrollView.ContentOffset.Y;

                nfloat topViewHeight = 0;
                if (this.topView != null)
                {
                    this.topView.Frame = new CGRect(0, -offset, this.Width, this.topView.Bounds.Size.Height);
                    topViewHeight = this.topView.Bounds.Size.Height;
                }

                this.Frame = new CGRect(-offset, -offset + topViewHeight, this.Width + (offset * 2), this.CoverViewHeight + offset);

                int index = (int)offset / 10;
                if (index < 0)
                {
                    index = 0;
                }
                else if (index >= this.blurredImages.Count)
                {
                    index = this.blurredImages.Count - 1;
                }

                var image = this.blurredImages[index];
                if (this.Image != image)
                {
                    base.Image = image;
                }
            }
            else
            {
                nfloat topViewHeight = 0;
                if (this.topView != null)
                {
                    this.topView.Frame = new CGRect(0, 0, this.Width, this.topView.Bounds.Size.Height);
                    topViewHeight = this.topView.Bounds.Size.Height;
                }

                this.Frame = new CGRect(0, topViewHeight, this.Width, this.CoverViewHeight);

                var image = this.blurredImages.FirstOrDefault();
                if (this.Image != image)
                {
                    base.Image = image;
                }
            }
        }

        /// <inheritdoc/>
        public override void RemoveFromSuperview()
        {
            this.InvokeOnMainThread(() =>
            {
                this.ScrollView?.RemoveObserver(this, "contentOffset");
                this.topView?.RemoveFromSuperview();
                base.RemoveFromSuperview();
            });
        }

        /// <inheritdoc/>
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            this.SetNeedsLayout();
        }

        private static UIImage? Blur(UIImage image, nfloat blurRadius)
        {
            if ((int)image.Size.Width < 1 || image.Size.Height < 1)
            {
                Debug.WriteLine(@"*** error: invalid size:({0} x {1}). Both dimensions must be >= 1: {2}", image.Size.Width, image.Size.Height, image);
                return null;
            }

            if (image.CGImage == null)
            {
                Debug.WriteLine(@"*** error: image must be backed by a CGImage: {0}", image);
                return null;
            }

            if (blurRadius < 0f || blurRadius > 1f)
            {
                blurRadius = 0.5f;
            }

            var inputRadius = blurRadius * UIScreen.MainScreen.Scale;
            var boxSize = (uint)(inputRadius * 40);
            boxSize = boxSize - (boxSize % 2) + 1;

            var imageRect = new CGRect(CGPoint.Empty, image.Size);

            UIImage effectImage;
            UIGraphics.BeginImageContextWithOptions(image.Size, false, UIScreen.MainScreen.Scale);
            {
                var contextIn = UIGraphics.GetCurrentContext();
                contextIn.DrawImage(imageRect, image.CGImage);
                var effectInContext = contextIn.AsBitmapContext();

                var effectInBuffer = new vImageBuffer
                {
                    Data = effectInContext.Data,
                    Width = (int)effectInContext.Width,
                    Height = (int)effectInContext.Height,
                    BytesPerRow = (int)effectInContext.BytesPerRow,
                };

                UIGraphics.BeginImageContextWithOptions(image.Size, false, UIScreen.MainScreen.Scale);
                {
                    var effectOutContext = UIGraphics.GetCurrentContext().AsBitmapContext();
                    var effectOutBuffer = new vImageBuffer
                    {
                        Data = effectOutContext.Data,
                        Width = (int)effectOutContext.Width,
                        Height = (int)effectOutContext.Height,
                        BytesPerRow = (int)effectOutContext.BytesPerRow,
                    };

                    vImage.BoxConvolveARGB8888(ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, boxSize, boxSize, Pixel8888.Zero, vImageFlags.EdgeExtend);
                    vImage.BoxConvolveARGB8888(ref effectOutBuffer, ref effectInBuffer, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, boxSize, boxSize, Pixel8888.Zero, vImageFlags.EdgeExtend);
                    vImage.BoxConvolveARGB8888(ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, boxSize, boxSize, Pixel8888.Zero, vImageFlags.EdgeExtend);

                    effectImage = UIGraphics.GetImageFromCurrentImageContext();

                    UIGraphics.EndImageContext();
                }

                UIGraphics.EndImageContext();
            }

            // Setup up output context
            UIImage outputImage;
            UIGraphics.BeginImageContextWithOptions(image.Size, false, UIScreen.MainScreen.Scale);
            {
                var outputContext = UIGraphics.GetCurrentContext();

                // Draw base image
                outputContext.SaveState();
                outputContext.DrawImage(imageRect, effectImage.CGImage);
                outputContext.RestoreState();

                outputImage = UIGraphics.GetImageFromCurrentImageContext();

                UIGraphics.EndImageContext();
            }

            return outputImage;
        }

        private void Initialize(UIView? top, nfloat coverViewHeight)
        {
            this.topView = top;
            this.CoverViewHeight = coverViewHeight;
            this.blurredImages = new List<UIImage>(20);
            this.ContentMode = UIViewContentMode.ScaleAspectFill;
            this.ClipsToBounds = true;
        }

        private void PrepareForBlurImages()
        {
            this.blurredImages.Clear();
            float num = 0.1f;
            if (this.Image is not null)
            {
                this.blurredImages.Add(this.Image);
            }

            for (int i = 0; i < 20; i++)
            {
                if (this.Image is not null)
                {
                    var img = Blur(this.Image, num);
                    if (img is not null)
                    {
                        this.blurredImages.Add(img);
                    }
                }

                num += 0.04f;
            }
        }
    }
}
