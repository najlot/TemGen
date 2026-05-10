using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Favorites;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase.Filters;
using <# Project.Namespace#>.Contracts.Favorites;
using <# Project.Namespace#>.Contracts.Filters;
using <# Project.Namespace#>.Contracts.Shared;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
using <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

namespace <# Project.Namespace#>.ClientBase.<# Definition.Name#>s;

public class <# Definition.Name#>sViewModel : ViewModelBase, IAsyncInitializable, IDisposable
{
	private readonly I<# Definition.Name#>Service _<# Definition.NameLow#>Service;
	private readonly IFavoriteService _favoriteService;
	private static readonly ItemType FavoriteTargetType = ItemType.<# Definition.Name#>;

	public bool IsBusy { get; set => Set(ref field, value, () => Filters.IsBusy = value); }
	public EntityFilterEditorViewModel Filters { get; }
	public ObservableCollection<<# Definition.Name#>ListItemViewModel> <# Definition.Name#>s { get; } = [];

	public <# Definition.Name#>sViewModel(
		I<# Definition.Name#>Service <# Definition.NameLow#>Service,
		IFavoriteService favoriteService,
		<# Definition.Name#>FilterViewModel filters,
		ViewModelBaseParameters<<# Definition.Name#>sViewModel> parameters) : base(parameters)
	{
		_<# Definition.NameLow#>Service = <# Definition.NameLow#>Service;
		_favoriteService = favoriteService;
		Filters = filters;

		Filters.FilterChanged += async (s, e) => await LoadItemsAsync(e);

		NavigateBackCommand = new AsyncCommand(() => NavigationService.NavigateBack(), t => HandleError(t.Exception));
		Add<# Definition.Name#>Command = new AsyncCommand(Add<# Definition.Name#>Async, t => HandleError(t.Exception));
		Edit<# Definition.Name#>Command = new AsyncCommand<<# Definition.Name#>ListItemViewModel>(Edit<# Definition.Name#>Async, t => HandleError(t.Exception));

		_<# Definition.NameLow#>Service.ItemCreated += Handle;
		_<# Definition.NameLow#>Service.ItemUpdated += Handle;
		_<# Definition.NameLow#>Service.ItemDeleted += Handle;
		_favoriteService.ItemCreated += HandleFavoriteCreated;
		_favoriteService.ItemDeleted += HandleFavoriteDeleted;
	}

	public AsyncCommand NavigateBackCommand { get; }
	public AsyncCommand<<# Definition.Name#>ListItemViewModel> Edit<# Definition.Name#>Command { get; }
	public AsyncCommand Add<# Definition.Name#>Command { get; }

	public async Task InitializeAsync()
	{
		await Task.WhenAll(
			_favoriteService.StartEventListener(),
			_<# Definition.NameLow#>Service.StartEventListener());
		await Filters.InitializeAsync();
	}

	private async Task LoadItemsAsync(EntityFilter? filter)
	{
		try
		{
			IsBusy = true;
			_lastFilter = filter;

			var itemsTask = filter is null || filter.Conditions.Count == 0
				? _<# Definition.NameLow#>Service.GetItemsAsync()
				: _<# Definition.NameLow#>Service.GetItemsAsync(filter);
			var favoritesTask = _favoriteService.GetItemsAsync(FavoriteTargetType);

			await Task.WhenAll(itemsTask, favoritesTask);

			var items = await itemsTask;
			var favoriteIds = (await favoritesTask)
				.Select(item => item.ItemId)
				.ToHashSet();

			var viewModels = Map.From<<# Definition.Name#>ListItemModel>(items).To<<# Definition.Name#>ListItemViewModel>();
			<# Definition.Name#>s.Clear();
			foreach (var item in viewModels)
			{
				item.IsFavorite = favoriteIds.Contains(item.Id);
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
			item.IsFavorite = false;
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

	private async Task HandleFavoriteCreated(object? sender, FavoriteCreated obj)
	{
		if (obj.TargetType != FavoriteTargetType)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => SetFavoriteState(obj.ItemId, true));
	}

	private async Task HandleFavoriteDeleted(object? sender, FavoriteDeleted obj)
	{
		if (obj.TargetType != FavoriteTargetType)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => SetFavoriteState(obj.ItemId, false));
	}

	private void SetFavoriteState(Guid id, bool isFavorite)
	{
		if (<# Definition.Name#>s.FirstOrDefault(item => item.Id == id) is { } item)
		{
			item.IsFavorite = isFavorite;
		}
	}

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
				_favoriteService.ItemCreated -= HandleFavoriteCreated;
				_favoriteService.ItemDeleted -= HandleFavoriteDeleted;
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