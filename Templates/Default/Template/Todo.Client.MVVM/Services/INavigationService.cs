using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.Services;

public interface INavigationService
{
	Task NavigateBack();

	Task NavigateForward(AbstractViewModel newViewModel);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>