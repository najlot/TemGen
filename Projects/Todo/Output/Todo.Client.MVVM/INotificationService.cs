using System.Threading.Tasks;

namespace Todo.Client.MVVM;

public interface INotificationService
{
	Task ShowErrorAsync(string message);
	Task ShowSuccessAsync(string message);
	Task ClearNotificationsAsync();
}