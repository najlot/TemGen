using System;
using System.Threading.Tasks;

namespace <#cs Write(Project.Namespace)#>.Client.MVVM;

public interface IDispatcherHelper
{
	Task InvokeOnUIThread(Action action);
	Task InvokeOnUIThread(Func<Task> action);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>