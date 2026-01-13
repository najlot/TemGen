using Todo.ClientBase.Models;
using System.Threading.Tasks;
using Cosei.Client.Base;
using Todo.ClientBase.Services.Implementation;
using Todo.Client.Data.Services;

namespace Todo.ClientBase.ProfileHandler;

public abstract class AbstractProfileHandler : IProfileHandler
{
	private IProfileHandler _handler = null;

	protected ISubscriber Subscriber { get; set; }

	protected INoteService NoteService { get; set; }
	protected NoteMessagingService NoteMessagingService { get; set; }
	protected ITodoItemService TodoItemService { get; set; }
	protected TodoItemMessagingService TodoItemMessagingService { get; set; }

	protected IUserService UserService { get; set; }
	protected UserMessagingService UserMessagingService { get; set; }
	public INoteService GetNoteService() => NoteService ?? _handler?.GetNoteService();
	public ITodoItemService GetTodoItemService() => TodoItemService ?? _handler?.GetTodoItemService();

	public IUserService GetUserService() => UserService ?? _handler?.GetUserService();
	public IProfileHandler SetNext(IProfileHandler handler) => _handler = handler;

	public async Task SetProfile(ProfileBase profile)
	{
		if (Subscriber != null)
		{
			await Subscriber.DisposeAsync();
			Subscriber = null;
		}

		NoteService?.Dispose();
		NoteService = null;
		NoteMessagingService?.Dispose();
		NoteMessagingService = null;
		TodoItemService?.Dispose();
		TodoItemService = null;
		TodoItemMessagingService?.Dispose();
		TodoItemMessagingService = null;

		UserService?.Dispose();
		UserService = null;
		UserMessagingService?.Dispose();
		UserMessagingService = null;

		await ApplyProfile(profile);

		if (_handler != null)
		{
			await _handler.SetProfile(profile);
		}
	}

	protected abstract Task ApplyProfile(ProfileBase profile);
}