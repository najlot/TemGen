using Todo.Client.Data.Services;
using Todo.ClientBase.Models;
using System.Threading.Tasks;

namespace Todo.ClientBase.ProfileHandler;

public interface IProfileHandler
{
	IUserService GetUserService();
	INoteService GetNoteService();
	ITodoItemService GetTodoItemService();

	IProfileHandler SetNext(IProfileHandler handler);

	Task SetProfile(ProfileBase profile);
}