using System;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class MenuViewModel : AbstractViewModel
{
	private readonly IErrorService _errorService;
	private readonly INavigationService _navigationService;
	private bool _isBusy = false;

<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"	private readonly All{definition.Name}sViewModel _all{definition.Name}sViewModel;");
}

WriteLine("");

foreach(var definition in Definitions.Where(d => !(d.IsArray
|| d.IsEnumeration
|| d.IsOwnedType
|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"	public AsyncCommand NavigateTo{definition.Name}s {{ get; }}");
	WriteLine($"	public async Task NavigateTo{definition.Name}sAsync()");
	WriteLine("	{");
	WriteLine("		if (_isBusy)");
	WriteLine("		{");
	WriteLine("			return;");
	WriteLine("		}");
	WriteLine("");
	WriteLine("		try");
	WriteLine("		{");
	WriteLine("			_isBusy = true;");
	WriteLine("");
	WriteLine($"			var refreshTask = _all{definition.Name}sViewModel.Refresh{definition.Name}sAsync();");
	WriteLine($"			await _navigationService.NavigateForward(_all{definition.Name}sViewModel);");
	WriteLine("			await refreshTask;");
	WriteLine("		}");
	WriteLine("		catch (Exception ex)");
	WriteLine("		{");
	WriteLine("			await _errorService.ShowAlertAsync(\"Could not load...\", ex);");
	WriteLine("		}");
	WriteLine("		finally");
	WriteLine("		{");
	WriteLine("			_isBusy = false;");
	WriteLine("		}");
	WriteLine("	}");
	WriteLine("");
}
#>	public MenuViewModel(IErrorService errorService,
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
	WriteLine($"		All{definition.Name}sViewModel all{definition.Name}sViewModel,");

Result = Result.TrimEnd();
#>
		INavigationService navigationService)
	{
		_errorService = errorService;
<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
	WriteLine($"		_all{definition.Name}sViewModel = all{definition.Name}sViewModel;");

Result = Result.TrimEnd();
#>
		_navigationService = navigationService;

<#cs
foreach(var definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase))))
{
	WriteLine($"		NavigateTo{definition.Name}s = new AsyncCommand(NavigateTo{definition.Name}sAsync, DisplayError);");
}
#>	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync("Error...", task.Exception);
	}
}<#cs SetOutputPathAndSkipOtherDefinitions()#>