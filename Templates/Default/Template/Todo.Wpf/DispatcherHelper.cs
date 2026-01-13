using System;
using System.Threading.Tasks;
using System.Windows;
using <#cs Write(Project.Namespace)#>.Client.MVVM;

namespace <#cs Write(Project.Namespace)#>.Wpf;

public class DispatcherHelper : IDispatcherHelper
{
	public void BeginInvokeOnMainThread(Action action)
	{
		Application.Current.Dispatcher.Invoke(action);
	}

	public async Task BeginInvokeOnMainThread(Func<Task> action)
	{
		await Application.Current.Dispatcher.Invoke(action);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>