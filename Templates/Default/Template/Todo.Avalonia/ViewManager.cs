using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using <# Project.Namespace#>.Avalonia.Views;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.ClientBase;
using <# Project.Namespace#>.ClientBase.GlobalSearch;
using <# Project.Namespace#>.ClientBase.History;
using <# Project.Namespace#>.ClientBase.Identity;
using <# Project.Namespace#>.ClientBase.Shared;
using <# Project.Namespace#>.ClientBase.Trash;
<#cs 
var features = Definitions
	.Where(e => !(e.IsArray 
				|| e.IsEnumeration 
				|| e.IsOwnedType 
				|| e.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
	.Distinct()
	.OrderByDescending(d => d.Name);

foreach (var feature in features)
{
	WriteLine($"using {Project.Namespace}.ClientBase.{feature.Name}s;");
}
#>
namespace <# Project.Namespace#>.Avalonia;

public class ViewManager(IServiceProvider serviceProvider) : IViewManager<Control>
{
	private readonly Dictionary<Type, Type> _knownControls = new()
	{
		[typeof(LoginViewModel)] = typeof(LoginView),
		[typeof(RegisterViewModel)] = typeof(RegisterView),
		[typeof(MenuViewModel)] = typeof(MenuView),
		[typeof(ManageViewModel)] = typeof(ManageView),
		[typeof(GlobalSearchViewModel)] = typeof(GlobalSearchView),
		[typeof(EntityHistoryViewModel)] = typeof(HistoryView),
		[typeof(TrashViewModel)] = typeof(TrashView),
<#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		[typeof(<# definition.Name#>sViewModel)] = typeof(<# definition.Name#>sView),
<#end
#><#for definition in Definitions.Where(d => !(d.IsArray
	|| d.IsEnumeration
	|| d.IsOwnedType
	|| d.Name.Equals("user", StringComparison.OrdinalIgnoreCase)))
#>		[typeof(<# definition.Name#>ViewModel)] = typeof(<# definition.Name#>View),
<#end#>	};

	public Control GetView<TViewModel>(TViewModel viewModel) where TViewModel : notnull
	{
		if (!_knownControls.TryGetValue(typeof(TViewModel), out var viewType))
		{
			throw new InvalidOperationException(
				$"No view is registered for view model type '{typeof(TViewModel).FullName}'. " +
				$"Ensure that this view model type is added to the '{nameof(ViewManager)}' {_knownControls.GetType().Name} mapping.");
		}

		if (viewType.GetConstructor(Type.EmptyTypes) is not null && Activator.CreateInstance(viewType) is Control control)
		{
			control.DataContext = viewModel;
			return control;
		}

		if (serviceProvider.GetRequiredService(viewType) is Control instance)
		{
			instance.DataContext = viewModel;
			return instance;
		}

		throw new InvalidOperationException($"The class {viewType.FullName} is not a control.");
	}

	public async Task<bool> CanNavigateAsync(Control? currentView)
	{
		if (currentView?.DataContext is INavigationGuard navigationGuard)
		{
			return await navigationGuard.CanNavigateAsync().ConfigureAwait(false);
		}

		return true;
	}

	public async Task DisposeView(Control? control)
	{
		await DisposeObject(control);
		await DisposeObject(control?.DataContext);
	}

	private static async Task DisposeObject(object? obj)
	{
		if (obj is IAsyncDisposable asyncDisposable)
		{
			await asyncDisposable.DisposeAsync();
		}
		else if (obj is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>