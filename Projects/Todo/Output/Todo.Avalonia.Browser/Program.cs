using Avalonia;
using Avalonia.Browser;
using System.Threading.Tasks;
using Todo.Avalonia;
using Todo.Avalonia.Browser.Identity;

internal sealed partial class Program
{
	private static Task Main(string[] args)
	{
		ServiceProviderFactory.PlatformUserDataStoreFactory ??= () => new WebUserDataStore();
		ServiceProviderFactory.PlatformDataServiceUrlFactory ??= BrowserRuntimeConfiguration.GetDataServiceUrl;

		return BuildAvaloniaApp()
			.WithInterFont()
			.StartBrowserAppAsync("out");
	}

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>();
}
