using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using <# Project.Namespace#>.Avalonia.Views;

namespace <# Project.Namespace#>.Avalonia;

public partial class App : Application
{
	public MainView? MainView { get; private set; }

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainWindow = new MainWindow();
			desktop.MainWindow = mainWindow;
			MainView = mainWindow.MainView;
		}
		else if (ApplicationLifetime is IActivityApplicationLifetime activityPlatform)
		{
			activityPlatform.MainViewFactory = () =>
			{
				var mainView = new MainView();
				MainView = mainView;
				return mainView;
			};
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		{
			var mainView = new MainView();
			singleViewPlatform.MainView = mainView;
			MainView = mainView;
		}

		base.OnFrameworkInitializationCompleted();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>