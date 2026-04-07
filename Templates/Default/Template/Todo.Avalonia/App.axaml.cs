using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using System.Linq;
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
			DisableAvaloniaDataAnnotationValidation();
			var mainWindow = new MainWindow();
			desktop.MainWindow = mainWindow;
			MainView = mainWindow.MainView;
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		{
			var mainView = new MainView();
			singleViewPlatform.MainView = mainView;
			MainView = mainView;
		}

		base.OnFrameworkInitializationCompleted();
	}

	private void DisableAvaloniaDataAnnotationValidation()
	{
		var dataValidationPluginsToRemove =
			BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

		foreach (var plugin in dataValidationPluginsToRemove)
		{
			BindingPlugins.DataValidators.Remove(plugin);
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>