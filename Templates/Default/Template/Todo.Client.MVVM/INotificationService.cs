using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.MVVM;

public interface INotificationService
{
	Task ShowErrorAsync(string message);
	Task ShowSuccessAsync(string message);
	Task ClearNotificationsAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>