using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Todo.Client.MVVM;

namespace Todo.Avalonia;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			action();
			return;
		}

		var completion = new TaskCompletionSource<object?>();

		Dispatcher.UIThread.Post(() =>
		{
			try
			{
				action();
				completion.SetResult(null);
			}
			catch (Exception ex)
			{
				completion.SetException(ex);
			}
		});

		await completion.Task;
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			await action();
			return;
		}

		var completion = new TaskCompletionSource<object?>();

		Dispatcher.UIThread.Post(async () =>
		{
			try
			{
				await action();
				completion.SetResult(null);
			}
			catch (Exception ex)
			{
				completion.SetException(ex);
			}
		});

		await completion.Task;
	}
}
