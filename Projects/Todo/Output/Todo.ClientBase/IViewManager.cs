using System.Threading.Tasks;

namespace Todo.ClientBase;

public interface IViewManager<TView>
{
	TView GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull;
	Task DisposeView(TView? control);
}
