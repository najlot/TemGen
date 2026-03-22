using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase;
using <# Project.Namespace#>.ClientBase.ViewModel;

namespace <# Project.Namespace#>.Avalonia.Views;

public partial class MainView : UserControl, INavigationService, INotificationService
{
	private readonly ViewStackNavigator<Control> _controlNavigator;
	private readonly IDispatcherHelper _dispatcherHelper;
	private static readonly Dictionary<string, object> _emptyParameters = [];

	public MainView()
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