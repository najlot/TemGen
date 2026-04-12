using System.Threading.Tasks;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.ClientBase.Trash;
using <# Project.Namespace#>.ClientBase.GlobalSearch;
<#cs 
var features = Definitions
	.Where(e => !(e.IsArray 
				|| e.IsEnumeration 
				|| e.IsOwnedType 
				|| e.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
	.Distinct()
	.OrderByDescending(d => d.Name);

foreach (var feature in features)
{
	WriteLine($"using {Project.Namespace}.ClientBase.{feature.Name}s;");
}
#>using <# Project.Namespace#>.ClientBase.Identity;

namespace <# Project.Namespace#>.ClientBase.Shared;

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
	private readonly IUserDataStore _userDataStore;
	private readonly ITokenProvider _tokenProvider;

	public MenuViewModel(
		IUserDataStore userDataStore,
		ITokenProvider tokenProvider,
		ViewModelBaseParameters<MenuViewModel> parameters) : base(parameters)
	{
		_userDataStore = userDataStore;
		_tokenProvider = tokenProvider;

		ToggleDrawerCommand = new RelayCommand(() => IsDrawerOpen = !IsDrawerOpen);
		NavigateToGlobalSearch = CreateNavigationCommand<GlobalSearchViewModel>();
		NavigateToTrash = CreateNavigationCommand<TrashViewModel>();
<#for definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		NavigateTo<# definition.Name#>s = CreateNavigationCommand<<# definition.Name#>sViewModel>();
<#end#>		ManageCommand = CreateNavigationCommand<ManageViewModel>();
		LogoutCommand = new AsyncCommand(LogoutAsync, t => HandleError(t.Exception));
	}

	private async Task LogoutAsync()
	{
		await _userDataStore.SetUserData(string.Empty, string.Empty);
		_tokenProvider.ClearCache();
		await NavigationService.NavigateForward<LoginViewModel>();
		IsDrawerOpen = false;
	}

	private AsyncCommand CreateNavigationCommand<TViewModel>() where TViewModel : notnull
		=> new(async () =>
		{
			await NavigationService.NavigateForward<TViewModel>();
			IsDrawerOpen = false;
		}, t => HandleError(t.Exception));
}<#cs SetOutputPathAndSkipOtherDefinitions()#>