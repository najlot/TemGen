namespace <#cs Write(Project.Namespace)#>.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
