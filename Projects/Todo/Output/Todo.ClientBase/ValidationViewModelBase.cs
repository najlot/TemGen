using Microsoft.Extensions.Logging;
using Najlot.Map;
using System;
using System.Threading.Tasks;
using Todo.Client.Localisation;
using Todo.Client.MVVM;

namespace Todo.ClientBase;

public abstract class ValidationViewModelBase(IViewModelBaseParameters parameters) : AbstractValidationViewModel
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
}
