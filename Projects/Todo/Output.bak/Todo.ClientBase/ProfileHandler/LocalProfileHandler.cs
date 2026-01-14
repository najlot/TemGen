using System.Threading.Tasks;
using Najlot.Map;
using Todo.Client.Data.Services.Implementation;
using Todo.Client.MVVM;
using Todo.ClientBase.Models;
using Todo.ClientBase.Services.Implementation;

namespace Todo.ClientBase.ProfileHandler;

public sealed class LocalProfileHandler : AbstractProfileHandler
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly IMap _map;

	public LocalProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IMap map)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_map = map;
	}

	protected override async Task ApplyProfile(ProfileBase profile)
	{
		if (profile is LocalProfile localProfile)
		{
			var subscriber = new LocalSubscriber();

			var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
			UserService = new UserService(userStore);
			UserMessagingService = new UserMessagingService(_messenger, _dispatcher, subscriber);
			var noteStore = new LocalNoteStore(localProfile.FolderName, subscriber, _map);
			NoteService = new NoteService(noteStore);
			NoteMessagingService = new NoteMessagingService(_messenger, _dispatcher, subscriber);
			var todoItemStore = new LocalTodoItemStore(localProfile.FolderName, subscriber, _map);
			TodoItemService = new TodoItemService(todoItemStore);
			TodoItemMessagingService = new TodoItemMessagingService(_messenger, _dispatcher, subscriber);

			await subscriber.StartAsync();

			Subscriber = subscriber;
		}
	}
}