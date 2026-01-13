using Cosei.Client.Base;
using Todo.Client.MVVM;
using Todo.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace Todo.ClientBase.Services.Implementation;

public class NoteMessagingService
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly ISubscriber _subscriber;

	public NoteMessagingService(
		IMessenger messenger,
		IDispatcherHelper dispatcher,
		ISubscriber subscriber)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_subscriber = subscriber;

		subscriber.Register<NoteCreated>(Handle);
		subscriber.Register<NoteUpdated>(Handle);
		subscriber.Register<NoteDeleted>(Handle);
	}

	private async Task Handle(NoteCreated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(NoteUpdated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(NoteDeleted message)
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