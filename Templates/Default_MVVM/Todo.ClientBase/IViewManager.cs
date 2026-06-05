using System.Threading.Tasks;

namespace <# Project.Namespace#>.ClientBase;

public interface IViewManager<TView>
{
	TView GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull;
	Task<bool> CanNavigateAsync(TView? currentView);
	Task DisposeView(TView? control);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>