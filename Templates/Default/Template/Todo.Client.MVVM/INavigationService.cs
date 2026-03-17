using System.Collections.Generic;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.MVVM;

public interface IParameterizable
{
	void SetParameters(IReadOnlyDictionary<string, object> parameters);
}

public interface IAsyncInitializable
{
	Task InitializeAsync();
}

public interface INavigationService
{
	Task NavigateForward<TViewModel>() where TViewModel : notnull;
	Task NavigateForward<TViewModel>(Dictionary<string, object> parameters) where TViewModel : notnull;
	Task NavigateBack();
}

public interface ISessionStart { }

public interface INavigationGuard
{
	Task<bool> CanNavigateAsync();
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
