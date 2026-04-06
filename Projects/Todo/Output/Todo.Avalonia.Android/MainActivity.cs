using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using Avalonia;
using Avalonia.Android;
using System;
using Todo.Avalonia.Android.Identity;
using Todo.Avalonia.Views;

namespace Todo.Avalonia.Android;

[Activity(
	Label = "Todo.Avalonia.Android",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	WindowSoftInputMode = SoftInput.AdjustResize,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		ServiceProviderFactory.PlatformUserDataStoreFactory ??= () => new SecureUserDataStore(this);
		base.OnCreate(savedInstanceState);
	}

	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return base.CustomizeAppBuilder(builder)
			.WithInterFont();
	}

	public override async void OnBackPressed()
	{
		try
		{
			if (global::Avalonia.Application.Current is App app
				&& app.MainView is MainView mainView)
			{
				if (mainView.CanNavigateBack())
				{
					await mainView.NavigateBack();
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error(typeof(MainActivity).FullName, $"OnBackPressed error {ex}");
		}

#pragma warning disable CA1422 // Validate platform compatibility
		base.OnBackPressed();
#pragma warning restore CA1422 // Validate platform compatibility
	}
}
