using Cosei.Client.Base;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;

public class UserMessagingService
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly ISubscriber _subscriber;

	public UserMessagingService(
		IMessenger messenger,
		IDispatcherHelper dispatcher,
		ISubscriber subscriber)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_subscriber = subscriber;

		subscriber.Register<UserCreated>(Handle);
		subscriber.Register<UserUpdated>(Handle);
		subscriber.Register<UserDeleted>(Handle);
	}

	private async Task Handle(UserCreated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(UserUpdated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(UserDeleted message)
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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>