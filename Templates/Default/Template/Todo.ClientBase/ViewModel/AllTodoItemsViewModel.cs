using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Localisation;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.Data.Services;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public class All<#cs Write(Definition.Name)#>sViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"	private readonly I{definition.Name}Service _{definition.NameLow}Service;");
}

#>	private readonly I<#cs Write(Definition.Name)#>Service _<#cs Write(Definition.NameLow)#>Service;

	public bool IsBusy { get; set => Set(ref field, value); }

	public string Filter
	{
		get;
		set => Set(ref field, value, () => <#cs Write(Definition.Name)#>sView.Refresh());
	} = string.Empty;

	public ObservableCollectionView<<#cs Write(Definition.Name)#>ListItemViewModel> <#cs Write(Definition.Name)#>sView { get; }
	public ObservableCollection<<#cs Write(Definition.Name)#>ListItemViewModel> <#cs Write(Definition.Name)#>s { get; } = [];

	public All<#cs Write(Definition.Name)#>sViewModel(
		I<#cs Write(Definition.Name)#>Service <#cs Write(Definition.NameLow)#>Service,
<#cs
				  foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
{
	WriteLine($"		I{definition.Name}Service {definition.NameLow}Service,");
}

#>		ViewModelBaseParameters <<#cs Write(Definition.Name)#>ViewModel> parameters) : base(parameters)
	{
		_<#cs Write(Definition.NameLow)#>Service = <#cs Write(Definition.NameLow)#>Service;
<#cs
foreach (var definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n)))
	{
		WriteLine($"		_{definition.NameLow}Service = {definition.NameLow}Service;");
	}
#>
		<#cs Write(Definition.Name)#>sView = new ObservableCollectionView<<#cs Write(Definition.Name)#>ListItemViewModel>(<#cs Write(Definition.Name)#>s, Filter<#cs Write(Definition.Name)#>);

		_<#cs Write(Definition.NameLow)#>Service.ItemCreated += Handle;
		_<#cs Write(Definition.NameLow)#>Service.ItemUpdated += Handle;
		_<#cs Write(Definition.NameLow)#>Service.ItemDeleted += Handle;

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		Add<#cs Write(Definition.Name)#>Command = new AsyncCommand(Add<#cs Write(Definition.Name)#>Async, t => HandleError(t.Exception));
		Edit<#cs Write(Definition.Name)#>Command = new AsyncCommand<<#cs Write(Definition.Name)#>ListItemViewModel>(Edit<#cs Write(Definition.Name)#>Async, t => HandleError(t.Exception));
		Refresh<#cs Write(Definition.Name)#>sCommand = new AsyncCommand(Refresh<#cs Write(Definition.Name)#>sAsync, t => HandleError(t.Exception));
	}

	public async Task InitializeAsync()
	{
		await Refresh<#cs Write(Definition.Name)#>sAsync();
		await _<#cs Write(Definition.NameLow)#>Service.StartEventListener();
	}

	private bool Filter<#cs Write(Definition.Name)#>(<#cs Write(Definition.Name)#>ListItemViewModel item)
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

	private async Task Handle(object? sender, <#cs Write(Definition.Name)#>Created obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			var item = Map.From(obj).To<<#cs Write(Definition.Name)#>ListItemViewModel>();
			<#cs Write(Definition.Name)#>s.Insert(0, item);
		});

	private async Task Handle(object? sender, <#cs Write(Definition.Name)#>Updated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (<#cs Write(Definition.Name)#>s.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				Map.From(obj).To(item);
			}
		});

	private async Task Handle(object? sender, <#cs Write(Definition.Name)#>Deleted obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (<#cs Write(Definition.Name)#>s.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				<#cs Write(Definition.Name)#>s.Remove(item);
			}
		});

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<<#cs Write(Definition.Name)#>ListItemViewModel> Edit<#cs Write(Definition.Name)#>Command { get; }
	public async Task Edit<#cs Write(Definition.Name)#>Async(<#cs Write(Definition.Name)#>ListItemViewModel? model)
	{
		if (IsBusy || model is null)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<<#cs Write(Definition.Name)#>ViewModel>(new() {{ "Id", model.Id }});
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoading} {ex.Message}");
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
			await NavigationService.NavigateForward<<#cs Write(Definition.Name)#>ViewModel>();
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorAdding} {ex.Message}");
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
			var viewModels = Map.From<<#cs Write(Definition.Name)#>ListItemModel>(<#cs Write(Definition.NameLow)#>s).To<<#cs Write(Definition.Name)#>ListItemViewModel>();

			foreach (var item in viewModels)
			{
				<#cs Write(Definition.Name)#>s.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoadingData} {ex.Message}");
		}
		finally
		{
			<#cs Write(Definition.Name)#>sView.Enable();
			IsBusy = false;
		}
	}

	private bool _disposedValue;
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_<#cs Write(Definition.NameLow)#>Service.ItemCreated -= Handle;
				_<#cs Write(Definition.NameLow)#>Service.ItemUpdated -= Handle;
				_<#cs Write(Definition.NameLow)#>Service.ItemDeleted -= Handle;
			}

			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>