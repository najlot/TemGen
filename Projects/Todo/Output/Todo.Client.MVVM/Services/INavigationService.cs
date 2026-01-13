using Todo.Client.MVVM.ViewModel;
using System.Threading.Tasks;

namespace Todo.Client.MVVM.Services;

public interface INavigationService
{
	Task NavigateBack();

	Task NavigateForward(AbstractViewModel newViewModel);
}