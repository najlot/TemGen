using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Client.Data.Services;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Contracts.Events;

namespace <# Project.Namespace#>.ClientBase.ViewModels;

public class <# Definition.Name#>sViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>	private readonly I<# definition.Name#>Service _<# definition.NameLow#>Service;
<#end#>	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;

	public bool IsBusy { get; set => Set(ref field, value); }

	public string Filter
	{
		get;
		set => Set(ref field, value, () => <# Definition.Name#>sView.Refresh());
	} = string.Empty;

	public ObservableCollectionView<<# Definition.Name#>ListItemViewModel> <# Definition.Name#>sView { get; }
	public ObservableCollection<<# Definition.Name#>ListItemViewModel> <# Definition.Name#>s { get; } = [];

	public <# Definition.Name#>sViewModel(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		I<# definition.Name#>Service <# definition.NameLow#>Service,
<#end#>		ViewModelBaseParameters <<# Definition.Name#>ViewModel> parameters) : base(parameters)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>		_<# definition.NameLow#>Service = <# definition.NameLow#>Service;
<#end#>
		<# Definition.Name#>sView = new ObservableCollectionView<<# Definition.Name#>ListItemViewModel>(<# Definition.Name#>s, Filter<# Definition.Name#>);

		_<# Definition.NameLow#>Service.ItemCreated += Handle;
		_<# Definition.NameLow#>Service.ItemUpdated += Handle;
		_<# Definition.NameLow#>Service.ItemDeleted += Handle;

<#cs if (Definition.Name == "Note") Write("\t\tNavigateBackCommand = new AsyncCommand(NavigationService.NavigateBack, t => HandleError(t.Exception));"); else Write("\t\tNavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));"); #>
		Add<# Definition.Name#>Command = new AsyncCommand(Add<# Definition.Name#>Async, t => HandleError(t.Exception));
		Edit<# Definition.Name#>Command = new AsyncCommand<<# Definition.Name#>ListItemViewModel>(Edit<# Definition.Name#>Async, t => HandleError(t.Exception));
	}

	public async Task InitializeAsync()
	{
		await Refresh<# Definition.Name#>sAsync();
		await _<# Definition.NameLow#>Service.StartEventListener();
	}

	private bool Filter<# Definition.Name#>(<# Definition.Name#>ListItemViewModel item)
	{
		if (string.IsNullOrEmpty(Filter))
		{
			return true;
		}

<#for entry in Entries
	.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2)
#><#if entry.EntryType == "string"
#>		var <# entry.FieldLow#> = item.<# entry.Field#>;
<#else
#>		var <# entry.FieldLow#> = item.<# entry.Field#>.ToString();
<#end
#>		if (!string.IsNullOrEmpty(<# entry.FieldLow#>) && <# entry.FieldLow#>.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) != -1)
		{
			return true;
		}

<#end#>		return false;
	}

	private async Task Handle(object? sender, <# Definition.Name#>Created obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			var item = Map.From(obj).To<<# Definition.Name#>ListItemViewModel>();
			<# Definition.Name#>s.Insert(0, item);
		});

	private async Task Handle(object? sender, <# Definition.Name#>Updated obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (<# Definition.Name#>s.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				Map.From(obj).To(item);
			}
		});

	private async Task Handle(object? sender, <# Definition.Name#>Deleted obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (<# Definition.Name#>s.FirstOrDefault(i => i.Id == obj.Id) is { } item)
			{
				<# Definition.Name#>s.Remove(item);
			}
		});

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<<# Definition.Name#>ListItemViewModel> Edit<# Definition.Name#>Command { get; }
	public async Task Edit<# Definition.Name#>Async(<# Definition.Name#>ListItemViewModel? model)
	{
		if (IsBusy || model is null)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<<# Definition.Name#>ViewModel>(new() {{ "Id", model.Id }});
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

	public AsyncCommand Add<# Definition.Name#>Command { get; }
	public async Task Add<# Definition.Name#>Async()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			await NavigationService.NavigateForward<<# Definition.Name#>ViewModel>();
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

	public async Task Refresh<# Definition.Name#>sAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;
			<# Definition.Name#>sView.Disable();
			Filter = "";

			<# Definition.Name#>s.Clear();

<#for definition in Entries.Where(e => e.IsReference).Select(e => e.ReferenceType).Distinct().Select(n => Definitions.First(d => d.Name == n))
#>			var <# definition.NameLow#>s = await _<# definition.NameLow#>Service.GetItemsAsync();
<#end#>			var <# Definition.NameLow#>s = await _<# Definition.NameLow#>Service.GetItemsAsync();
			var viewModels = Map.From<<# Definition.Name#>ListItemModel>(<# Definition.NameLow#>s).To<<# Definition.Name#>ListItemViewModel>();

			foreach (var item in viewModels)
			{
				<# Definition.Name#>s.Add(item);
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorLoadingData} {ex.Message}");
		}
		finally
		{
			<# Definition.Name#>sView.Enable();
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
				_<# Definition.NameLow#>Service.ItemCreated -= Handle;
				_<# Definition.NameLow#>Service.ItemUpdated -= Handle;
				_<# Definition.NameLow#>Service.ItemDeleted -= Handle;
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