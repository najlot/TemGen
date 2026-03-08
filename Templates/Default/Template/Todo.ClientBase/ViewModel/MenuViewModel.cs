using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class MenuViewModel : ViewModelBase
{
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"	public AsyncCommand NavigateTo{definition.Name}s {{ get; }}");
}
#>	public AsyncCommand ManageCommand { get; }
	public AsyncCommand LogoutCommand { get; }

	public MenuViewModel(ViewModelBaseParameters<MenuViewModel> parameters) : base(parameters)
	{
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		NavigateTo{definition.Name}s = CreateNavigationCommand<All{definition.Name}sViewModel>();");
}
#>		ManageCommand = CreateNavigationCommand<ManageViewModel>();
		LogoutCommand = CreateNavigationCommand<LoginViewModel>();
	}

	private AsyncCommand CreateNavigationCommand<TViewModel>() where TViewModel : notnull
		=> new(NavigationService.NavigateForward<TViewModel>, t => HandleError(t.Exception));
}<#cs SetOutputPathAndSkipOtherDefinitions()#>