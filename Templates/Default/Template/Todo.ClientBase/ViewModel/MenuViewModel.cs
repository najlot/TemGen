using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public class MenuViewModel : ViewModelBase
{
	public bool IsDrawerOpen { get; set => Set(ref field, value); }
	public RelayCommand ToggleDrawerCommand { get; }
	public AsyncCommand NavigateToGlobalSearch { get; }
	public AsyncCommand NavigateToTrash { get; }

<#for definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>	public AsyncCommand NavigateTo<# definition.Name#>s { get; }
<#end#>	public AsyncCommand ManageCommand { get; }
	public AsyncCommand LogoutCommand { get; }

	public MenuViewModel(ViewModelBaseParameters<MenuViewModel> parameters) : base(parameters)
	{
		ToggleDrawerCommand = new RelayCommand(() => IsDrawerOpen = !IsDrawerOpen);
		NavigateToGlobalSearch = CreateNavigationCommand<GlobalSearchViewModel>();
		NavigateToTrash = CreateNavigationCommand<TrashViewModel>();
<#for definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		NavigateTo<# definition.Name#>s = CreateNavigationCommand<All<# definition.Name#>sViewModel>();
<#end#>		ManageCommand = CreateNavigationCommand<ManageViewModel>();
		LogoutCommand = CreateNavigationCommand<LoginViewModel>();
	}

	private AsyncCommand CreateNavigationCommand<TViewModel>() where TViewModel : notnull
		=> new(async () =>
		{
			await NavigationService.NavigateForward<TViewModel>();
			IsDrawerOpen = false;
		}, t => HandleError(t.Exception));
}<#cs SetOutputPathAndSkipOtherDefinitions()#>