using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.ClientBase;

public interface IViewManager<TView>
{
	TView GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull;
	Task DisposeView(TView? control);
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>