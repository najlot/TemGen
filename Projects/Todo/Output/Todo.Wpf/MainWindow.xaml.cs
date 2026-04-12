using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Todo.Client.Data.Identity;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.ClientBase.Identity;
using Todo.ClientBase.Shared;

namespace Todo.Wpf;

public partial class MainWindow : Window, INavigationService, INotificationService
{
	private readonly ViewStackNavigator<Control> _controlNavigator;
	private readonly IDispatcherHelper _dispatcherHelper;
	private readonly ITokenProvider _tokenProvider;
	private readonly IUserDataStore _userDataStore;

	public MainWindow()
	{
		InitializeComponent();

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
			if (await HasValidSession())
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

	public bool CanNavigateBack() => _controlNavigator.CanNavigateBack();

	public async Task NavigateBack()
	{
		await ClearNotificationsAsync();
		await _controlNavigator.NavigateBack();
		ContentHost.Content = _controlNavigator.CurrentView;
	}

	private readonly static Dictionary<string, object> _emptyParameters = [];
	public async Task NavigateForward<TViewModel>() where TViewModel : notnull => await NavigateForward<TViewModel>(_emptyParameters);

	public async Task NavigateForward<TViewModel>(Dictionary<string, object> parameters) where TViewModel : notnull
	{
		await ClearNotificationsAsync();
		await _controlNavigator.NavigateForward<TViewModel>(parameters);
		ContentHost.Content = _controlNavigator.CurrentView;
	}

	public async Task ClearNotificationsAsync()
		=> await _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.Visibility = Visibility.Collapsed;
			ErrorText.Text = string.Empty;

			SuccessBorder.Visibility = Visibility.Collapsed;
			SuccessText.Text = string.Empty;
		});

	public async Task ShowErrorAsync(string message)
		=> await _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.Visibility = Visibility.Visible;
			ErrorText.Text = message;

			SuccessBorder.Visibility = Visibility.Collapsed;
			SuccessText.Text = string.Empty;
		});

	public async Task ShowSuccessAsync(string message)
		=> await _dispatcherHelper.InvokeOnUIThread(() =>
		{
			ErrorBorder.Visibility = Visibility.Collapsed;
			ErrorText.Text = string.Empty;

			SuccessBorder.Visibility = Visibility.Visible;
			SuccessText.Text = message;
		});
}