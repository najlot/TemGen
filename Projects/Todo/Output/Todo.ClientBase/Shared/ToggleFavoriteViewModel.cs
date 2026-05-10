using System;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Favorites;
using Todo.Client.Localisation;
using Todo.Client.MVVM;
using Todo.Contracts.Shared;

namespace Todo.ClientBase.Shared;

public sealed class ToggleFavoriteViewModel : ViewModelBase, IDisposable
{
	private readonly IFavoriteService _favoriteService;
	private ItemType _targetType;
	private Guid _itemId;
	private bool _isConfigured;
	private bool _disposedValue;

	public ToggleFavoriteViewModel(
		IFavoriteService favoriteService,
		ViewModelBaseParameters<ToggleFavoriteViewModel> parameters) : base(parameters)
	{
		_favoriteService = favoriteService;
		ToggleFavoriteCommand = new AsyncCommand(ToggleFavoriteAsync, t => HandleError(t.Exception), () => CanToggle);

		_favoriteService.ItemCreated += HandleFavoriteCreated;
		_favoriteService.ItemDeleted += HandleFavoriteDeleted;
	}

	public bool IsFavorite
	{
		get;
		private set => Set(ref field, value, () => RaisePropertyChanged(nameof(FavoriteIconKind)));
	}

	public string FavoriteIconKind => IsFavorite ? "Star" : "StarBorder";

	public bool CanToggle
	{
		get;
		private set => Set(ref field, value, ToggleFavoriteCommand.RaiseCanExecuteChanged);
	}

	public AsyncCommand ToggleFavoriteCommand { get; }

	public async Task InitializeAsync(ItemType targetType, Guid itemId, bool canToggle)
	{
		_targetType = targetType;
		_isConfigured = true;

		await _favoriteService.StartEventListener();

		UpdateState(itemId, canToggle);

		if (_itemId == Guid.Empty)
		{
			IsFavorite = false;
			return;
		}

		var favorites = await _favoriteService.GetItemsAsync(_targetType);
		IsFavorite = favorites.Any(item => item.ItemId == _itemId);
	}

	public void UpdateState(Guid itemId, bool canToggle)
	{
		_itemId = itemId;
		CanToggle = canToggle;

		if (_itemId == Guid.Empty)
		{
			IsFavorite = false;
		}
	}

	private async Task ToggleFavoriteAsync()
	{
		if (!CanToggle || _itemId == Guid.Empty)
		{
			return;
		}

		try
		{
			if (IsFavorite)
			{
				await _favoriteService.DeleteItemAsync(_targetType, _itemId);
				IsFavorite = false;
			}
			else
			{
				await _favoriteService.AddItemAsync(_targetType, _itemId);
				IsFavorite = true;
			}
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorSaving} {ex.Message}");
		}
	}

	private async Task HandleFavoriteCreated(object? sender, Contracts.Favorites.FavoriteCreated obj)
	{
		if (!_isConfigured || obj.TargetType != _targetType || obj.ItemId != _itemId)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => IsFavorite = true);
	}

	private async Task HandleFavoriteDeleted(object? sender, Contracts.Favorites.FavoriteDeleted obj)
	{
		if (!_isConfigured || obj.TargetType != _targetType || obj.ItemId != _itemId)
		{
			return;
		}

		await DispatcherHelper.InvokeOnUIThread(() => IsFavorite = false);
	}

	private void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
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
}
