using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.Uno;

public class DispatcherHelper : IDispatcherHelper
{
	private readonly DispatcherQueue _dispatcherQueue;

	public DispatcherHelper()
	{
		_dispatcherQueue = DispatcherQueue.GetForCurrentThread();
	}

	public Task InvokeOnUIThread(Action action)
	{
		var tcs = new TaskCompletionSource();
		_dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () =>
		{
			action();
			tcs.SetResult();
		});
		return tcs.Task;
	}

	public Task InvokeOnUIThread(Func<Task> action)
	{
		var tcs = new TaskCompletionSource();
		_dispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, async () =>
		{
			await action();
			tcs.SetResult();
		});
		return tcs.Task;
	}
}

