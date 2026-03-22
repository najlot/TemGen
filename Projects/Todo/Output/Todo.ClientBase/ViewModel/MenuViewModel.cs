using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModel;

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

	public MenuViewModel(ViewModelBaseParameters<MenuViewModel> parameters) : base(parameters)
	{
		ToggleDrawerCommand = new RelayCommand(() => IsDrawerOpen = !IsDrawerOpen);
		NavigateToGlobalSearch = CreateNavigationCommand<GlobalSearchViewModel>();
		NavigateToTrash = CreateNavigationCommand<TrashViewModel>();
		NavigateToNotes = CreateNavigationCommand<AllNotesViewModel>();
		NavigateToTodoItems = CreateNavigationCommand<AllTodoItemsViewModel>();
		ManageCommand = CreateNavigationCommand<ManageViewModel>();
		LogoutCommand = CreateNavigationCommand<LoginViewModel>();
	}

	private AsyncCommand CreateNavigationCommand<TViewModel>() where TViewModel : notnull
		=> new(async () =>
		{
			await NavigationService.NavigateForward<TViewModel>();
			IsDrawerOpen = false;
		}, t => HandleError(t.Exception));
}