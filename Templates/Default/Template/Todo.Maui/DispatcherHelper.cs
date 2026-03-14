using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.Maui;

public class DispatcherHelper : IDispatcherHelper
{
	public async Task InvokeOnUIThread(Action action)
	{
		await MainThread.InvokeOnMainThreadAsync(action);
	}

	public async Task InvokeOnUIThread(Func<Task> action)
	{
		await MainThread.InvokeOnMainThreadAsync(action);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
