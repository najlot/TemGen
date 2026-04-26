using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using Avalonia.Android;
using System;
using <# Project.Namespace#>.Avalonia.Android.Identity;
using <# Project.Namespace#>.Avalonia.Views;

namespace <# Project.Namespace#>.Avalonia.Android;

[Activity(
	Label = "<# Project.Namespace#>.Avalonia.Android",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	WindowSoftInputMode = SoftInput.AdjustResize,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		ServiceProviderFactory.PlatformUserDataStoreFactory ??= () => new SecureUserDataStore(this);
		base.OnCreate(savedInstanceState);
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

		if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
		{
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
			base.OnBackPressed();
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
			return;
		}

		Finish();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>