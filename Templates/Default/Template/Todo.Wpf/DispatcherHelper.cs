using System;
using System.Threading.Tasks;
using System.Windows;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.Wpf;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		await Application.Current.Dispatcher.InvokeAsync(action);
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await Application.Current.Dispatcher.InvokeAsync(action);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>