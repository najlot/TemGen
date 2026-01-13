using Cosei.Client.Base;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Services.Implementation;

public class <#cs Write(Definition.Name)#>MessagingService
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly ISubscriber _subscriber;

	public <#cs Write(Definition.Name)#>MessagingService(
		IMessenger messenger,
		IDispatcherHelper dispatcher,
		ISubscriber subscriber)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_subscriber = subscriber;

		subscriber.Register<<#cs Write(Definition.Name)#>Created>(Handle);
		subscriber.Register<<#cs Write(Definition.Name)#>Updated>(Handle);
		subscriber.Register<<#cs Write(Definition.Name)#>Deleted>(Handle);
	}

	private async Task Handle(<#cs Write(Definition.Name)#>Created message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(<#cs Write(Definition.Name)#>Updated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(<#cs Write(Definition.Name)#>Deleted message)
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
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>