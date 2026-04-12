using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.Client.Data.Identity;
using Todo.ClientBase.Trash;
using Todo.ClientBase.GlobalSearch;
using Todo.ClientBase.TodoItems;
using Todo.ClientBase.Notes;
using Todo.ClientBase.Identity;

namespace Todo.ClientBase.Shared;

public class MenuViewModel : ViewModelBase
{
	public bool IsDrawerOpen { get; set => Set(ref field, value); }
	public RelayCommand ToggleDrawerCommand { get; }
	public AsyncCommand NavigateToGlobalSearch { get; }
	public AsyncCommand NavigateToTrash { get; }

	public AsyncCommand NavigateToNotes { get; }
	public AsyncCommand NavigateToTodoItems { get; }
	public AsyncCommand ManageCommand { get; }
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
		NavigateToNotes = CreateNavigationCommand<NotesViewModel>();
		NavigateToTodoItems = CreateNavigationCommand<TodoItemsViewModel>();
		ManageCommand = CreateNavigationCommand<ManageViewModel>();
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
}