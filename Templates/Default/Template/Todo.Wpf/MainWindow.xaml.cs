using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.ClientBase;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

namespace <#cs Write(Project.Namespace)#>.Wpf;

public partial class MainWindow : Window, INavigationService, INotificationService
{
	private readonly ViewStackNavigator<Control> _controlNavigator;
	private readonly IDispatcherHelper _dispatcherHelper;

	public MainWindow()
	{
		InitializeComponent();

		var serviceProvider = ServiceProviderFactory.CreateServiceProvider(this, this);
		_dispatcherHelper = serviceProvider.GetRequiredService<IDispatcherHelper>();
		_controlNavigator = new ViewStackNavigator<Control>(serviceProvider);

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
}<#cs SetOutputPathAndSkipOtherDefinitions()#>