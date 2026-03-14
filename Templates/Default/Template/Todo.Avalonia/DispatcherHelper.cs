using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.Avalonia;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		await Dispatcher.UIThread.InvokeAsync(action);
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await Dispatcher.UIThread.InvokeAsync(action);
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
