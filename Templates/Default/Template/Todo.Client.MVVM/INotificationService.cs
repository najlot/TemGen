using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM;

public interface INotificationService
{
	Task ShowErrorAsync(string message);
	Task ShowSuccessAsync(string message);
	Task ClearNotificationsAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>