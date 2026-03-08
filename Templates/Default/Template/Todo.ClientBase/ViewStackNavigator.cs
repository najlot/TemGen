using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public sealed class ViewStackNavigator<TView>(ServiceProvider serviceProvider)
{
	private readonly IDispatcherHelper _dispatcherHelper = serviceProvider.GetRequiredService<IDispatcherHelper>();
	private readonly IViewManager<TView> _viewManager = serviceProvider.GetRequiredService<IViewManager<TView>>();

	private AsyncServiceScope _sessionScope = serviceProvider.CreateAsyncScope();
	
	private readonly Stack<TView> _navigationStack = [];

	public TView? CurrentView { get; private set; }

	public bool CanNavigateBack() => _navigationStack.Count > 0;

	public async Task NavigateBack()
	{
		if (!CanNavigateBack())
		{
			return;
		}

		await _dispatcherHelper.InvokeOnUIThread(async () =>
		{
			if (!CanNavigateBack())
			{
				return;
			}

			await _viewManager.DisposeView(CurrentView);
			CurrentView = _navigationStack.Pop();
		});
	}

	public async Task NavigateForward<TViewModel>(Dictionary<string, object> parameters) where TViewModel : notnull
	{
		bool isSessionStart = typeof(TViewModel).IsAssignableTo(typeof(ISessionStart));
		if (isSessionStart)
		{
			await StartNewSession();
		}

		var vm = _sessionScope.ServiceProvider.GetRequiredService<TViewModel>();
		
		if (vm is IParameterizable parameterizable)
		{
			parameterizable.SetParameters(parameters);
		}

		if (vm is IAsyncInitializable asyncInitializable)
		{
			await asyncInitializable.InitializeAsync();
		}

		await _dispatcherHelper.InvokeOnUIThread(() =>
		{
			if (CurrentView is TView view)
			{
				_navigationStack.Push(view);
			}

			CurrentView = _viewManager.GetView(vm);
		});

		if (isSessionStart)
		{
			while(_navigationStack.TryPop(out TView? view))
			{
				await _viewManager.DisposeView(view);
			}
		}
	}

	private async Task StartNewSession()
	{
		await _sessionScope.DisposeAsync();
		_sessionScope = serviceProvider.CreateAsyncScope();
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>