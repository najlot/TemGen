using System;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM;

public interface IDispatcherHelper
{
	void BeginInvokeOnMainThread(Action action);
	Task BeginInvokeOnMainThread(Func<Task> action);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>