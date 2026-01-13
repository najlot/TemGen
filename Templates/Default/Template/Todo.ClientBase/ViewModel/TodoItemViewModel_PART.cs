using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.Client.MVVM;
using <#cs Write(Project.Namespace)#>.Client.MVVM.Services;
using <#cs Write(Project.Namespace)#>.Client.MVVM.ViewModel;

namespace <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;

public partial class <#cs Write(Definition.Name)#>ViewModel
{
	private ObservableCollection<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> _<#cs Write(DefinitionEntry?.FieldLow)#> = [];
	public ObservableCollection<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> <#cs Write(DefinitionEntry?.Field)#> { get => _<#cs Write(DefinitionEntry?.FieldLow)#>; set => Set(nameof(<#cs Write(DefinitionEntry?.Field)#>), ref _<#cs Write(DefinitionEntry?.FieldLow)#>, value); }

	public RelayCommand Add<#cs Write(DefinitionEntry?.EntryType)#>Command => new(Add<#cs Write(DefinitionEntry?.EntryType)#>);
	private void Add<#cs Write(DefinitionEntry?.EntryType)#>()
	{
		var max = 0;

		if (<#cs Write(DefinitionEntry?.Field)#>.Count > 0)
		{
			max = <#cs Write(DefinitionEntry?.Field)#>.Max(e => e.Id) + 1;
		}

		var model = new <#cs Write(DefinitionEntry?.EntryType)#>Model() { Id = max };
		var viewModel = _map.From(model).To<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel>();
		viewModel.Id = max;
		viewModel.ParentId = Id;

		<#cs Write(DefinitionEntry?.Field)#>.Add(viewModel);
	}

	public AsyncCommand<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> Edit<#cs Write(DefinitionEntry?.EntryType)#>Command => new(Edit<#cs Write(DefinitionEntry?.EntryType)#>, DisplayError);
	private async Task Edit<#cs Write(DefinitionEntry?.EntryType)#>(<#cs Write(DefinitionEntry?.EntryType)#>ViewModel vm)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var viewModel = _map.From(vm).To<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel>();
			viewModel.ParentId = Id;

			viewModel.OnSaveRequested(Save<#cs Write(DefinitionEntry?.EntryType)#>Async);
			viewModel.OnDeleteRequested(Delete<#cs Write(DefinitionEntry?.EntryType)#>Async);

			await _navigationService.NavigateForward(viewModel);
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error loading...", ex);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task Save<#cs Write(DefinitionEntry?.EntryType)#>Async(<#cs Write(DefinitionEntry?.EntryType)#>ViewModel viewModel)
	{
		try
		{
			var vm = <#cs Write(DefinitionEntry?.Field)#>.FirstOrDefault(i => i.Id == viewModel.Id);
			_map.From(viewModel).ToNullable(vm);

			await _navigationService.NavigateBack();
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
	}

	public AsyncCommand<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> Delete<#cs Write(DefinitionEntry?.EntryType)#>Command => new(Delete<#cs Write(DefinitionEntry?.EntryType)#>Async, DisplayError);
	private async Task<bool> Delete<#cs Write(DefinitionEntry?.EntryType)#>Async(<#cs Write(DefinitionEntry?.EntryType)#>ViewModel viewModel)
	{
		if (IsBusy)
		{
			return false;
		}

		try
		{
			IsBusy = true;

			var yesNoPageViewModel = new YesNoPageViewModel()
			{
				Title = "Delete?",
				Message = "Should the item be deleted?"
			};

			var selection = await _navigationService.RequestInputAsync(yesNoPageViewModel);

			if (selection)
			{
				var oldItem = <#cs Write(DefinitionEntry?.Field)#>.FirstOrDefault(i => i.Id == viewModel.Id);

				if (oldItem != null)
				{
					var index = <#cs Write(DefinitionEntry?.Field)#>.IndexOf(oldItem);

					if (index != -1)
					{
						<#cs Write(DefinitionEntry?.Field)#>.RemoveAt(index);
					}
				}
			}

			return selection;
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error deleting...", ex);
		}
		finally
		{
			IsBusy = false;
		}

		return false;
	}
}<#cs
if (DefinitionEntry != null && DefinitionEntry.IsArray)
{
	RelativePath = RelativePath.Replace("TodoItemViewModel_PART", $"{Definition.Name}ViewModel_{DefinitionEntry.Field}").Replace("Todo", Project.Namespace);
}
else
{
	RepeatForEachDefinitionEntry = !(Definition.IsOwnedType || Definition.IsEnumeration) && Entries.Where(e => e.IsArray).Any();
	RelativePath = "";
}
#>