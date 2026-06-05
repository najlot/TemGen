using System;
using System.Threading.Tasks;

namespace <# Project.Namespace#>.Client.MVVM;

public interface IDispatcherHelper
{
	Task InvokeOnUIThread(Action action);
	Task InvokeOnUIThread(Func<Task> action);
}<#cs SetOutputPathAndSkipOtherDefinitions()#>