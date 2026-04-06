namespace Todo.Service.Features.Auth;

public interface IUserIdProvider
{
	Guid GetRequiredUserId();
	string GetRequiredUsername();
}
