using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Client.MVVM;
using Todo.ClientBase;
using Todo.ClientBase.ViewModel;

namespace Todo.Uno;

public sealed partial class MainPage : UserControl, INavigationService, INotificationService
{
	private readonly ViewStackNavigator<FrameworkElement> _controlNavigator;
	private readonly IDispatcherHelper _dispatcherHelper;
	private readonly DispatcherQueue _dispatcherQueue;

	public MainPage()
	{
		this.InitializeComponent();

		_dispatcherQueue = DispatcherQueue.GetForCurrentThread();

		var serviceProvider = ServiceProviderFactory.CreateServiceProvider(this, this);
		_dispatcherHelper = serviceProvider.GetRequiredService<IDispatcherHelper>();
		_controlNavigator = new ViewStackNavigator<FrameworkElement>(serviceProvider);

		NavigateToLogin();
	}

	private async void NavigateToLogin()
	{
		try
		{
			await NavigateForward<LoginViewModel>();
		}
		catch (Exception ex)
		{
			await ShowErrorAsync($"An error occurred while navigating: {ex}");
		}
	}

	public bool CanNavigateBack() => _controlNavigator.CanNavigateBack();

	public async Task NavigateBack()
	{
		await ClearNotificationsAsync();
		await _controlNavigator.NavigateBack();
		ContentHost.Content = _controlNavigator.CurrentView;
	}

	private static readonly Dictionary<string, object> _emptyParameters = [];
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

