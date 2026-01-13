using System;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

public interface IPopupViewModel { }

public abstract class AbstractPopupViewModel<T> : AbstractViewModel, IPopupViewModel
{
	public Action<T> SetResult { get; set; }
}<#cs SetOutputPathAndSkipOtherDefinitions()#>