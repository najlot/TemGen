using Cosei.Client.Base;
using Cosei.Client.Http;
using Najlot.Map;
using System;
using System.Threading.Tasks;
using Todo.ClientBase.Models;
using Todo.ClientBase.Services.Implementation;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.Data.Services.Implementation;
using Todo.Client.Data.Repositories.Implementation;
using Todo.Client.Data.Identity;

namespace Todo.ClientBase.ProfileHandler;

public sealed class RestProfileHandler : AbstractProfileHandler
{
	private RestProfile _profile;
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly IErrorService _errorService;
	private readonly IMap _map;

	public RestProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IErrorService errorService, IMap map)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_errorService = errorService;
		_map = map;
	}

	private IRequestClient CreateRequestClient()
	{
		return new HttpRequestClient(_profile.ServerName);
	}

	protected override async Task ApplyProfile(ProfileBase profile)
	{
		if (profile is RestProfile restProfile)
		{
			_profile = restProfile;

			var requestClient = CreateRequestClient();
			var tokenProvider = new TokenProvider(CreateRequestClient, _profile.ServerUser, _profile.ServerPassword);

			var token = await tokenProvider.GetToken();

			var serverUri = new Uri(_profile.ServerName);
			var signalRUri = new Uri(serverUri, "/cosei");

			var subscriber = new SignalRSubscriber(signalRUri.AbsoluteUri,
				options =>
				{
					options.Headers.Add("Authorization", $"Bearer {token}");
				},
				exception =>
				{
					_dispatcher.BeginInvokeOnMainThread(async () => await _errorService.ShowAlertAsync(exception));
				});

			var userStore = new UserRepository(requestClient, tokenProvider, _map);
			UserService = new UserService(userStore);
			UserMessagingService = new UserMessagingService(_messenger, _dispatcher, subscriber);
			var noteStore = new NoteRepository(requestClient, tokenProvider, _map);
			NoteService = new NoteService(noteStore);
			NoteMessagingService = new NoteMessagingService(_messenger, _dispatcher, subscriber);
			var todoItemStore = new TodoItemRepository(requestClient, tokenProvider, _map);
			TodoItemService = new TodoItemService(todoItemStore);
			TodoItemMessagingService = new TodoItemMessagingService(_messenger, _dispatcher, subscriber);

			await subscriber.StartAsync();

			Subscriber = subscriber;
		}
	}
}