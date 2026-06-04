using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using Avalonia.Android;
using Microsoft.Extensions.Configuration;
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
public class MainActivity : AvaloniaMainActivity
{
	private static string GetDataServiceUrl()
	{
		var assembly = typeof(ServiceProviderFactory).Assembly;
		using var appSettingsStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json")
			?? throw new InvalidOperationException("Embedded appsettings.json not found.");

		var configurationBuilder = new ConfigurationBuilder()
			.AddJsonStream(appSettingsStream);

		var configuration = configurationBuilder.Build();

		return configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");
	}

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		ServiceProviderFactory.PlatformUserDataStoreFactory ??= () => new SecureUserDataStore(this);
		ServiceProviderFactory.PlatformDataServiceUrlFactory ??= () => GetDataServiceUrl();
		
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
