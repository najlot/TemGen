using System;

namespace Todo.Client.MVVM.ViewModel;

public interface IPopupViewModel { }

public abstract class AbstractPopupViewModel<T> : AbstractViewModel, IPopupViewModel
{
	public Action<T> SetResult { get; set; }
}