using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.Avalonia;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		await Dispatcher.UIThread.InvokeAsync(action);
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await Dispatcher.UIThread.InvokeAsync(action);
	}
}

