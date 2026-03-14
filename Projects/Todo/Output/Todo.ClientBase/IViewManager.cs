using System.Threading.Tasks;

namespace Todo.ClientBase;

public interface IViewManager<TView>
{
	TView GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull;
	Task<bool> CanNavigateAsync(TView? currentView);
	Task DisposeView(TView? control);
}
