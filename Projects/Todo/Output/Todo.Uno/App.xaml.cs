using Microsoft.UI.Xaml;

namespace Todo.Uno;

public sealed partial class App : Application
{
	private Window? AppWindow { get; set; }

	public App()
	{
		this.InitializeComponent();
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var window = new Window();
		AppWindow = window;

		var mainPage = new MainPage();
		window.Content = mainPage;

		window.Activate();
	}
}

