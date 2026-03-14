using System;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.Maui;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		await MainThread.InvokeOnMainThreadAsync(action);
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await MainThread.InvokeOnMainThreadAsync(action);
	}
}
