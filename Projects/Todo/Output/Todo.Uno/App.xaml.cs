using Microsoft.UI.Xaml;

namespace Todo.Uno;

public sealed partial class App : Application
{
	protected Window? MainWindow { get; private set; }

	public App()
	{
		this.InitializeComponent();
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var window = new Window();
		MainWindow = window;

		var mainPage = new MainPage();
		window.Content = mainPage;

		window.Activate();
	}
}

