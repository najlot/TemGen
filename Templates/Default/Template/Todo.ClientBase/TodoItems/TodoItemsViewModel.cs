using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase.Filters;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
using <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

namespace <# Project.Namespace#>.ClientBase.<# Definition.Name#>s;

public class <# Definition.Name#>sViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<<# Definition.Name#>ListItemViewModel> <# Definition.Name#>s { get; } = [];

	public <# Definition.Name#>sViewModel(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
		<# Definition.Name#>FilterViewModel filters,
		ViewModelBaseParameters<<# Definition.Name#>sViewModel> parameters) : base(parameters)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		Add<# Definition.Name#>Command = new AsyncCommand(Add<# Definition.Name#>Async, t => HandleError(t.Exception));
		Edit<# Definition.Name#>Command = new AsyncCommand<<# Definition.Name#>ListItemViewModel>(Edit<# Definition.Name#>Async, t => HandleError(t.Exception));

		_<# Definition.NameLow#>Service.ItemCreated += Handle;
		_<# Definition.NameLow#>Service.ItemUpdated += Handle;
		_<# Definition.NameLow#>Service.ItemDeleted += Handle;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<<# Definition.Name#>ListItemViewModel> Edit<# Definition.Name#>Command { get; }
	public AsyncCommand Add<# Definition.Name#>Command { get; }

	public async Task InitializeAsync()
	{
		await Filters.InitializeAsync();
		await _<# Definition.NameLow#>Service.StartEventListener();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var items = filter is null || filter.Conditions.Count == 0
				? await _<# Definition.NameLow#>Service.GetItemsAsync()
				: await _<# Definition.NameLow#>Service.GetItemsAsync(filter);

			var viewModels = Map.From<<# Definition.Name#>ListItemModel>(items).To<<# Definition.Name#>ListItemViewModel>();
			<# Definition.Name#>s.Clear();
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
			IsBusy = false;
		}
	}

	private EntityFilter? _lastFilter;

	private async Task Handle(object? sender, <# Definition.Name#>Created obj)
		=> await DispatcherHelper.InvokeOnUIThread(() =>
		{
			if (_lastFilter is { Conditions.Count: > 0 })
			{
				return;
			}

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