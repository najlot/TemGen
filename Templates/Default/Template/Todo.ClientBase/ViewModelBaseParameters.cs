using Microsoft.Extensions.Logging;
using Najlot.Map;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase;

public interface IViewModelBaseParameters
{
	INavigationService NavigationService { get; }
	INotificationService NotificationService { get; }
	IDispatcherHelper DispatcherHelper { get; }
	IMap Map { get; }
	ILogger Logger { get; }
}

public class ViewModelBaseParameters<TViewModel>(
	INavigationService navigationService,
	INotificationService notificationService,
	IDispatcherHelper dispatcherHelper,
	IMap map,
	ILogger<TViewModel> logger) : IViewModelBaseParameters
{
	public INavigationService NavigationService { get; } = navigationService;
	public INotificationService NotificationService { get; } = notificationService;
	public IDispatcherHelper DispatcherHelper { get; } = dispatcherHelper;
	public IMap Map { get; } = map;
	public ILogger Logger { get; } = logger;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
