using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Najlot.Map;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class All<#cs Write(Definition.Name)#>sViewModel : AbstractViewModel, IDisposable
{
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"	private readonly I{definition.Name}Service _{definition.NameLow}Service;");
}

#>	private readonly IErrorService _errorService;
	private readonly I<#cs Write(Definition.Name)#>Service _<#cs Write(Definition.NameLow)#>Service;
	private readonly INavigationService _navigationService;
	private readonly IMessenger _messenger;
	private readonly IMap _map;

	private bool _isBusy;
	private string _filter;

	public bool IsBusy
	{
		get => _isBusy;
		set => Set(nameof(IsBusy), ref _isBusy, value);
	}

	public string Filter
	{
		get => _filter;
		set
		{
			Set(nameof(Filter), ref _filter, value);
			<#cs Write(Definition.Name)#>sView.Refresh();
		}
	}

	public ObservableCollectionView<<#cs Write(Definition.Name)#>ListItemModel> <#cs Write(Definition.Name)#>sView { get; }
	public ObservableCollection<<#cs Write(Definition.Name)#>ListItemModel> <#cs Write(Definition.Name)#>s { get; } = [];

	public All<#cs Write(Definition.Name)#>sViewModel(
		IErrorService errorService,
<#cs 
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"		I{definition.Name}Service {definition.NameLow}Service,");
}

Write("		I" + Definition.Name)
#>Service <#cs Write(Definition.NameLow)#>Service,
		INavigationService navigationService,
		IMessenger messenger,
		IMap map)
	{
		_errorService = errorService;
<#cs 
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"		_{definition.NameLow}Service = {definition.NameLow}Service;");
}

Write("		_" + Definition.NameLow)
			#>Service = <#cs Write(Definition.NameLow)#>Service;
		_navigationService = navigationService;
		_messenger = messenger;
		_map = map;

		<#cs Write(Definition.Name)#>sView = new ObservableCollectionView<<#cs Write(Definition.Name)#>ListItemModel>(<#cs Write(Definition.Name)#>s, Filter<#cs Write(Definition.Name)#>);

		_messenger.Register<<#cs Write(Definition.Name)#>Created>(Handle);
		_messenger.Register<<#cs Write(Definition.Name)#>Updated>(Handle);
		_messenger.Register<<#cs Write(Definition.Name)#>Deleted>(Handle);

		Add<#cs Write(Definition.Name)#>Command = new AsyncCommand(Add<#cs Write(Definition.Name)#>Async, DisplayError);
		Edit<#cs Write(Definition.Name)#>Command = new AsyncCommand<<#cs Write(Definition.Name)#>ListItemModel>(Edit<#cs Write(Definition.Name)#>Async, DisplayError);
		Refresh<#cs Write(Definition.Name)#>sCommand = new AsyncCommand(Refresh<#cs Write(Definition.Name)#>sAsync, DisplayError);
	}

	private bool Filter<#cs Write(Definition.Name)#>(<#cs Write(Definition.Name)#>ListItemModel item)
	{
		if (string.IsNullOrEmpty(Filter))
		{
			return true;
		}

<#cs
foreach (var entry in Entries
	.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	if (entry.EntryType == "string")
	{
		WriteLine("		var " + entry.FieldLow + " = item." + entry.Field + ";");
	}
	else
	{
		WriteLine("		var " + entry.FieldLow + " = item." + entry.Field + ".ToString();");
	}

	WriteLine("		if (!string.IsNullOrEmpty("+entry.FieldLow+") && "+entry.FieldLow+".IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)");
	WriteLine("		{");
	WriteLine("			return true;");
	WriteLine("		}");
	WriteLine("");
}

#>		return false;
	}

	private async Task DisplayError(Task task)
	{
		await _errorService.ShowAlertAsync(CommonLoc.Error, task.Exception);
	}

	private void Handle(<#cs Write(Definition.Name)#>Created obj)
	{
		<#cs Write(Definition.Name)#>s.Insert(0, new <#cs Write(Definition.Name)#>ListItemModel()
		{
			Id = obj.Id,
<#cs
foreach(var entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	if (entry.IsReference)
	{
		WriteLine($"			{entry.Field}Id = obj.{entry.Field}Id,");
	}
	else
	{
		WriteLine($"			{entry.Field} = obj.{entry.Field},");
	}
}

Result = Result.TrimEnd('\r', '\n');
#>
		});
	}

	private void Handle(<#cs Write(Definition.Name)#>Updated obj)
	{
		var oldItem = <#cs Write(Definition.Name)#>s.FirstOrDefault(i => i.Id == obj.Id);
		var index = -1;

		if (oldItem != null)
		{
			index = <#cs Write(Definition.Name)#>s.IndexOf(oldItem);

			if (index != -1)
			{
				<#cs Write(Definition.Name)#>s.RemoveAt(index);
			}
		}

		if (index == -1)
		{
			index = 0;
		}

		<#cs Write(Definition.Name)#>s.Insert(index, new <#cs Write(Definition.Name)#>ListItemModel()
		{
			Id = obj.Id,
<#cs
foreach(var entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	if (entry.IsReference)
	{
		WriteLine($"			{entry.Field}Id = obj.{entry.Field}Id,");
	}
	else
	{
		WriteLine($"			{entry.Field} = obj.{entry.Field},");
	}
}

Result = Result.TrimEnd('\r', '\n');
#>
		});
	}

	private void Handle(<#cs Write(Definition.Name)#>Deleted obj)
	{
		var oldItem = <#cs Write(Definition.Name)#>s.FirstOrDefault(i => i.Id == obj.Id);

		if (oldItem != null)
		{
			<#cs Write(Definition.Name)#>s.Remove(oldItem);
		}
	}

	public AsyncCommand<<#cs Write(Definition.Name)#>ListItemModel> Edit<#cs Write(Definition.Name)#>Command { get; }
	public async Task Edit<#cs Write(Definition.Name)#>Async(<#cs Write(Definition.Name)#>ListItemModel model)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var item = await _<#cs Write(Definition.NameLow)#>Service.GetItemAsync(model.Id);
			var viewModel = _map.From(item).To<<#cs Write(Definition.Name)#>ViewModel>();

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand Add<#cs Write(Definition.Name)#>Command { get; }
	public async Task Add<#cs Write(Definition.Name)#>Async()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var item = _<#cs Write(Definition.NameLow)#>Service.Create<#cs Write(Definition.Name)#>();
			var viewModel = _map.From(item).To<<#cs Write(Definition.Name)#>ViewModel>();
			viewModel.IsNew = true;

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error adding...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	public AsyncCommand Refresh<#cs Write(Definition.Name)#>sCommand { get; }
	public async Task Refresh<#cs Write(Definition.Name)#>sAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			<#cs Write(Definition.Name)#>sView.Disable();
			Filter = "";

			<#cs Write(Definition.Name)#>s.Clear();

<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"			var {definition.NameLow}s = await _{definition.NameLow}Service.GetItemsAsync();");
}
#>			var <#cs Write(Definition.NameLow)#>s = await _<#cs Write(Definition.NameLow)#>Service.GetItemsAsync();

			foreach (var item in <#cs Write(Definition.NameLow)#>s)
			{
				<#cs Write(Definition.Name)#>s.Add(item);
			}
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading data...", ex);
		}
		finally
		{
			<#cs Write(Definition.Name)#>sView.Enable();
			IsBusy = false;
		}
	}

	public void Dispose() => _messenger.Unregister(this);
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>