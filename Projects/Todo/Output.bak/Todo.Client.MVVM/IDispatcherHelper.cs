using System;
using System.Threading.Tasks;

namespace Todo.Client.MVVM;

public interface IDispatcherHelper
{
	void BeginInvokeOnMainThread(Action action);
	Task BeginInvokeOnMainThread(Func<Task> action);
}