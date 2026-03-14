using Microsoft.Extensions.Logging;
using Najlot.Map;
using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public abstract class ViewModelBase(IViewModelBaseParameters parameters) : AbstractViewModel
{
	protected INavigationService NavigationService { get; } = parameters.NavigationService;
	protected INotificationService NotificationService { get; } = parameters.NotificationService;
	protected IDispatcherHelper DispatcherHelper { get; } = parameters.DispatcherHelper;
	protected IMap Map { get; } = parameters.Map;
	protected ILogger Logger { get; } = parameters.Logger;

	protected virtual async Task HandleError(Exception? ex)
	{
		Logger.LogError(ex, "An error occurred");
		await DispatcherHelper.InvokeOnUIThread(() => NotificationService.ShowErrorAsync(ex?.Message ?? ErrorLoc.ErrorCouldNotLoad));
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
