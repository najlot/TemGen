using System;
using System.Threading.Tasks;
using System.Windows;
using Todo.Client.MVVM;

namespace Todo.Wpf;

public class DispatcherHelper : IDispatcherHelper
{
	public Task InvokeOnUIThread(Action action)
	{
		Application.Current.Dispatcher.Invoke(action);
		return Task.CompletedTask;
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await Application.Current.Dispatcher.Invoke(action);
	}
}