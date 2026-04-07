using UIKit;

namespace <# Project.Namespace#>.Avalonia.iOS;

public class Application
{
	static void Main(string[] args)
	{
		UIApplication.Main(args, null, typeof(AppDelegate));
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>