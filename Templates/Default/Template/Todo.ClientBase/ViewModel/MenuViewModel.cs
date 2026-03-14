using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class MenuViewModel : ViewModelBase
{
	public bool IsDrawerOpen { get; set => Set(ref field, value); }
	public RelayCommand ToggleDrawerCommand { get; }

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
		ToggleDrawerCommand = new RelayCommand(() => IsDrawerOpen = !IsDrawerOpen);
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
		=> new(async () =>
		{
			await NavigationService.NavigateForward<TViewModel>();
			IsDrawerOpen = false;
		}, t => HandleError(t.Exception));
}<#cs SetOutputPathAndSkipOtherDefinitions()#>