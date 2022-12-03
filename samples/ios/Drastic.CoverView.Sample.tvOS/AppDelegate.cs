// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.CoverView.Sample;

namespace Drastic.CoverView.Sample.tvOS;

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
        this.Window.RootViewController = new UINavigationController(new RootViewController());
        this.Window.MakeKeyAndVisible();

        return true;
    }
}
