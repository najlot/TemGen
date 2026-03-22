using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.Avalonia.ViewModels;

public partial class MainViewModel : AbstractViewModel
{
	public string Greeting => "Welcome to Avalonia!";
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>