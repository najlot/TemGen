namespace Todo.Service.Services;

public interface IUserIdProvider
{
	Guid GetRequiredUserId();
}
