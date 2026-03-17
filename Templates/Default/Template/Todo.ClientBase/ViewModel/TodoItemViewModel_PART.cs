using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Client.MVVM;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public partial class <# Definition.Name#>ViewModel
{
	public ObservableCollection<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> <#cs Write(DefinitionEntry?.Field)#> { get; set => Set(ref field, value); } = [];

	private void Initialize<#cs Write(DefinitionEntry?.Field)#>Tracking()
	{
		foreach (var item in <#cs Write(DefinitionEntry?.Field)#>)
		{
			item.ChangeVisitor = _changeTracker;
		}
	}

	public RelayCommand Add<#cs Write(DefinitionEntry?.EntryType)#>Command => new(Add<#cs Write(DefinitionEntry?.EntryType)#>);
	private void Add<#cs Write(DefinitionEntry?.EntryType)#>()
	{
		var max = 0;

		if (<#cs Write(DefinitionEntry?.Field)#>.Count > 0)
		{
			max = <#cs Write(DefinitionEntry?.Field)#>.Max(e => e.Id) + 1;
		}

		var model = new <#cs Write(DefinitionEntry?.EntryType)#>Model() { Id = max };
		var viewModel = Map.From(model).To<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel>();
		viewModel.Id = max;
		viewModel.ParentId = Id;
		viewModel.ChangeVisitor = _changeTracker;

		<#cs Write(DefinitionEntry?.Field)#>.Add(viewModel);

		_changeTracker.Track(
			nameof(<#cs Write(DefinitionEntry?.Field)#>),
			() => <#cs Write(DefinitionEntry?.Field)#>.Remove(viewModel),
			() =>
			{
				viewModel.ChangeVisitor = _changeTracker;
				<#cs Write(DefinitionEntry?.Field)#>.Add(viewModel);
			});
	}

	public AsyncCommand<<#cs Write(DefinitionEntry?.EntryType)#>ViewModel> Delete<#cs Write(DefinitionEntry?.EntryType)#>Command => new(Delete<#cs Write(DefinitionEntry?.EntryType)#>Async, t => HandleError(t.Exception));
	private async Task<bool> Delete<#cs Write(DefinitionEntry?.EntryType)#>Async(<#cs Write(DefinitionEntry?.EntryType)#>ViewModel? viewModel)
	{
		if (IsBusy || viewModel is null)
		{
			return false;
		}

		try
		{
			IsBusy = true;

			var oldItem = <#cs Write(DefinitionEntry?.Field)#>.FirstOrDefault(i => i.Id == viewModel.Id);

			if (oldItem is not null)
			{
				var index = <#cs Write(DefinitionEntry?.Field)#>.IndexOf(oldItem);

				if (index != -1)
				{
					<#cs Write(DefinitionEntry?.Field)#>.RemoveAt(index);

					_changeTracker.Track(
						nameof(<#cs Write(DefinitionEntry?.Field)#>),
						() =>
						{
							oldItem.ChangeVisitor = _changeTracker;
							<#cs Write(DefinitionEntry?.Field)#>.Insert(index, oldItem);
						},
						() => <#cs Write(DefinitionEntry?.Field)#>.Remove(oldItem));
				}
			}

			return true;
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync($"{ErrorLoc.ErrorDeleting} {ex.Message}");
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
