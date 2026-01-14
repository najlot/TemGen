using Cosei.Client.Base;
using Todo.Client.MVVM;
using Todo.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace Todo.ClientBase.Services.Implementation;

public class TodoItemMessagingService
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly ISubscriber _subscriber;

	public TodoItemMessagingService(
		IMessenger messenger,
		IDispatcherHelper dispatcher,
		ISubscriber subscriber)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_subscriber = subscriber;

		subscriber.Register<TodoItemCreated>(Handle);
		subscriber.Register<TodoItemUpdated>(Handle);
		subscriber.Register<TodoItemDeleted>(Handle);
	}

	private async Task Handle(TodoItemCreated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(TodoItemUpdated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(TodoItemDeleted message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private bool _disposedValue = false;

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			if (disposing)
			{
				_subscriber.Unregister(this);
			}
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}