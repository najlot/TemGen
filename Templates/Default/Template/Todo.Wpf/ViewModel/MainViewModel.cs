using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

namespace <#cs Write(Project.Namespace)#>.Wpf.ViewModel;

public class MainViewModel : AbstractViewModel, INavigationService
{
	private AbstractViewModel _viewModel;
	private readonly Stack<AbstractViewModel> _backViewModels = [];
	private AbstractViewModel _lastViewModel = null;

	private bool _isPopup = false;

	public AsyncCommand NavigateBackCommand { get; }

	public AbstractViewModel ViewModel
	{
		get => _viewModel;
		private set
		{
			_isPopup = value is IPopupViewModel;
			Set(nameof(ViewModel), ref _viewModel, value);
		}
	}

	public MainViewModel()
	{
		NavigateBackCommand = new AsyncCommand(NavigateBack, DebugFailExceptionAsync, () => _backViewModels.Count > 0 && !_isPopup);
	}

	private Task DebugFailExceptionAsync(Task task)
	{
		var exception = task.Exception;
		Debug.Fail(exception.ToString());
		return Task.CompletedTask;
	}

	public Task NavigateForward(AbstractViewModel newViewModel)
	{
		if (_lastViewModel != null)
		{
			_backViewModels.Push(_lastViewModel);
		}

		_lastViewModel = newViewModel;

		ViewModel = newViewModel;

		NavigateBackCommand.RaiseCanExecuteChanged();

		return Task.CompletedTask;
	}

	public Task NavigateBack()
	{
		if (_backViewModels.Count < 1)
		{
			return Task.CompletedTask;
		}

		_lastViewModel = _backViewModels.Pop();

		if (_lastViewModel != null)
		{
			ViewModel = _lastViewModel;
		}

		NavigateBackCommand.RaiseCanExecuteChanged();

		return Task.CompletedTask;
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>