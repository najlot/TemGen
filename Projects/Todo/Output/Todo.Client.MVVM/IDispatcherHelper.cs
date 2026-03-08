using System;
using System.Threading.Tasks;

namespace Todo.Client.MVVM;

public interface IDispatcherHelper
{
	Task InvokeOnUIThread(Action action);
	Task InvokeOnUIThread(Func<Task> action);
}