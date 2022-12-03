// <copyright file="CoverViewExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Drastic.CoverView
{
    internal enum ObjcAssociationPolicy
    {
        Assign = 0,
        RetainNonatomic = 1,
        CopyNonatomic = 3,
        Retain = 01401,
        Copy = 01403,
    }

    public static class CoverViewExtensions
    {
        private static readonly NSObject CoverViewKey = new NSObject();

        private enum AssociationPolicy
        {
            ASSIGN,
            RETAIN_NONATOMIC,
            COPY_NONATOMIC = 3,
            RETAIN = 1401,
            COPY = 1403,
        }

        public static CoverView AddCoverImage(this UIScrollView scrollView, CoverView coverImageView)
        {
            return AddCoverImage(scrollView, coverImageView, null);
        }

        public static CoverView AddCoverImage(this UIScrollView scrollView, CoverView coverImageView, UIView? topView = default)
        {
            scrollView.AddSubview(coverImageView);
            if (topView != null)
            {
                scrollView.AddSubview(topView);
            }

            scrollView.SetCoverView(coverImageView);

            return coverImageView;
        }

        public static CoverView AddCoverImage(this UIScrollView scrollView, UIImage image, UIView? topView = default, nfloat? coverViewHeight = default, UIColor? backgroundColor = default)
        {
            var coverImageView = new CoverView(topView, coverViewHeight)
            {
                BackgroundColor = backgroundColor ?? UIColor.Clear,
                Image = image,
                ScrollView = scrollView,
            };

            return AddCoverImage(scrollView, coverImageView);
        }

        public static CoverView AddCoverImage(this UIScrollView scrollView, UIImage image, nfloat? coverViewHeight = default, UIColor? backgroundColor = default)
        {
            return scrollView.AddCoverImage(image, null, coverViewHeight, backgroundColor);
        }

        public static void RemoveCoverImage(this UIScrollView scrollView)
        {
            scrollView.GetCoverView().RemoveFromSuperview();
            scrollView.SetCoverView(null);
        }

        public static CoverView GetCoverView(this UIScrollView scrollView)
        {
            return scrollView.GetAssociatedObject(CoverViewKey.Handle) as CoverView ?? throw new NullReferenceException();
        }

        public static void SetCoverView(this UIScrollView scrollView, CoverView? coverView)
        {
            scrollView.SetAssociatedObject(CoverViewKey.Handle, coverView, ObjcAssociationPolicy.Retain);
        }
    }

    internal static class ObjCExtensions
    {
        public static void SetAssociatedObject(this NSObject obj, IntPtr key, NSObject? value, ObjcAssociationPolicy policy = ObjcAssociationPolicy.RetainNonatomic)
        {
            objc_setAssociatedObject(obj.Handle, key, value?.Handle ?? IntPtr.Zero, policy);
            GC.KeepAlive(value);
            GC.KeepAlive(obj);
        }

        public static NSObject? GetAssociatedObject(this NSObject obj, IntPtr key)
        {
            var result = Runtime.GetNSObject(objc_getAssociatedObject(obj.Handle, key));
            GC.KeepAlive(obj);
            return result;
        }

        [DllImport(Constants.ObjectiveCLibrary)]
        static extern void objc_setAssociatedObject(IntPtr obj, IntPtr key, IntPtr value, ObjcAssociationPolicy policy);

        [DllImport(Constants.ObjectiveCLibrary)]
        static extern IntPtr objc_getAssociatedObject(IntPtr obj, IntPtr key);
    }
}