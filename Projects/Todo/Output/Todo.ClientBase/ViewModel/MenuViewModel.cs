using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModel;

public class MenuViewModel : ViewModelBase
{
	public AsyncCommand NavigateToNotes { get; }
	public AsyncCommand NavigateToTodoItems { get; }
	public AsyncCommand ManageCommand { get; }
	public AsyncCommand LogoutCommand { get; }

	public MenuViewModel(ViewModelBaseParameters<MenuViewModel> parameters) : base(parameters)
	{
		NavigateToNotes = CreateNavigationCommand<AllNotesViewModel>();
		NavigateToTodoItems = CreateNavigationCommand<AllTodoItemsViewModel>();
		ManageCommand = CreateNavigationCommand<ManageViewModel>();
		LogoutCommand = CreateNavigationCommand<LoginViewModel>();
	}

	private AsyncCommand CreateNavigationCommand<TViewModel>() where TViewModel : notnull
		=> new(NavigationService.NavigateForward<TViewModel>, t => HandleError(t.Exception));
}