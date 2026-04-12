using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Identity;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase;
using <# Project.Namespace#>.ClientBase.Identity;
using <# Project.Namespace#>.ClientBase.Shared;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class MainView : UserControl, INavigationService, INotificationService
{
	private readonly ViewStackNavigator<Control> _controlNavigator;
	private readonly IDispatcherHelper _dispatcherHelper;
	private readonly ITokenProvider _tokenProvider;
	private readonly IUserDataStore _userDataStore;
	private static readonly Dictionary<string, object> _emptyParameters = [];
	private TopLevel? _topLevel;

	public MainView()
	{
		InitializeComponent();
		TopLevel.SetAutoSafeAreaPadding(this, true);
		AttachedToVisualTree += (_, _) => AttachTopLevel();
		DetachedFromVisualTree += (_, _) => DetachTopLevel();

		var serviceProvider = ServiceProviderFactory.CreateServiceProvider(this, this);
		_dispatcherHelper = serviceProvider.GetRequiredService<IDispatcherHelper>();
		_tokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
		_userDataStore = serviceProvider.GetRequiredService<IUserDataStore>();
		_controlNavigator = new ViewStackNavigator<Control>(serviceProvider);

		NavigateToInitialView();
	}

	private async void NavigateToInitialView()
	{
		try
		{
			if (await HasValidSession().ConfigureAwait(true))
			{
				await NavigateForward<MenuViewModel>();
				return;
			}

			await NavigateForward<LoginViewModel>();
		}
		catch (Exception ex)
		{
			await ShowErrorAsync($"An error occurred while navigating: {ex}");
		}
	}

	private async Task<bool> HasValidSession()
	{
		try
		{
			var token = await _tokenProvider.GetToken();
			return !string.IsNullOrWhiteSpace(token);
		}
		catch (SessionUnavailableException)
		{
			_tokenProvider.ClearCache();
			await _userDataStore.SetUserData(string.Empty, string.Empty);
			return false;
		}
		catch (ArgumentException)
		{
			_tokenProvider.ClearCache();
			await _userDataStore.SetUserData(string.Empty, string.Empty);
			return false;
		}
	}

	private void AttachTopLevel()
	{
		var topLevel = TopLevel.GetTopLevel(this);
		if (ReferenceEquals(_topLevel, topLevel))
		{
			return;
		}

		DetachTopLevel();
		_topLevel = topLevel;
		if (_topLevel is not null)
		{
			_topLevel.BackRequested += OnBackRequested;
		}
	}

	private void DetachTopLevel()
	{
		if (_topLevel is null)
		{
			return;
		}

		_topLevel.BackRequested -= OnBackRequested;
		_topLevel = null;
	}

	private async void OnBackRequested(object? sender, RoutedEventArgs e)
	{
		if (!CanNavigateBack())
		{
			return;
		}

		e.Handled = true;

		try
		{
			await NavigateBack();
		}
		catch (Exception ex)
		{
			await ShowErrorAsync($"An error occurred while navigating back: {ex}");
		}
	}

	public bool CanNavigateBack() => _controlNavigator.CanNavigateBack();

	public async Task NavigateBack()
	{
		await ClearNotificationsAsync();
		await _controlNavigator.NavigateBack();
		ContentHost.Content = _controlNavigator.CurrentView;
	}

	public Task NavigateForward<TViewModel>() where TViewModel : notnull => NavigateForward<TViewModel>(_emptyParameters);

	public async Task NavigateForward<TViewModel>(Dictionary<string, object> parameters) where TViewModel : notnull
	{
		await ClearNotificationsAsync();
		await _controlNavigator.NavigateForward<TViewModel>(parameters);
		ContentHost.Content = _controlNavigator.CurrentView;
	}

	public Task ClearNotificationsAsync()
		=> _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.IsVisible = false;
			ErrorText.Text = string.Empty;

			SuccessBorder.IsVisible = false;
			SuccessText.Text = string.Empty;
		});

	public Task ShowErrorAsync(string message)
		=> _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.IsVisible = true;
			ErrorText.Text = message;

			SuccessBorder.IsVisible = false;
			SuccessText.Text = string.Empty;
		});

	public Task ShowSuccessAsync(string message)
		=> _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.IsVisible = false;
			ErrorText.Text = string.Empty;

			SuccessBorder.IsVisible = true;
			SuccessText.Text = message;
		});
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>