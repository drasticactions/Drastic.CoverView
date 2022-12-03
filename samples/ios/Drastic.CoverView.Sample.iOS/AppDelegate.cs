// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.CoverView.Sample;

namespace Drastic.CoverView.Sample.iOS;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    /// <inheritdoc/>
    public override UIWindow? Window
    {
        get;
        set;
    }

    /// <inheritdoc/>
    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        this.Window = new UIWindow(UIScreen.MainScreen.Bounds);

        var test = new UINavigationController(new RootViewController());
        var appearance = new UINavigationBarAppearance();
        appearance.ConfigureWithDefaultBackground();

        // test.ChildView
        test.NavigationBar.StandardAppearance = appearance;

        // test.NavigationBar.ScrollEdgeAppearance = appearance;
        // test.NavigationBar.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.5f);
        this.Window.RootViewController = test;
        this.Window.MakeKeyAndVisible();

        return true;
    }
}
