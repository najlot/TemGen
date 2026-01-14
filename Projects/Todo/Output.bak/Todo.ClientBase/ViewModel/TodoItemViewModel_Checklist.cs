using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.MVVM;
using Todo.Client.MVVM.Services;
using Todo.Client.MVVM.ViewModel;

namespace Todo.ClientBase.ViewModel;

public partial class TodoItemViewModel
{
	private ObservableCollection<ChecklistTaskViewModel> _checklist = [];
	public ObservableCollection<ChecklistTaskViewModel> Checklist { get => _checklist; set => Set(nameof(Checklist), ref _checklist, value); }

	public RelayCommand AddChecklistTaskCommand => new(AddChecklistTask);
	private void AddChecklistTask()
	{
		var max = 0;

		if (Checklist.Count > 0)
		{
			max = Checklist.Max(e => e.Id) + 1;
		}

		var model = new ChecklistTaskModel() { Id = max };
		var viewModel = _map.From(model).To<ChecklistTaskViewModel>();
		viewModel.Id = max;
		viewModel.ParentId = Id;

		Checklist.Add(viewModel);
	}

	public AsyncCommand<ChecklistTaskViewModel> EditChecklistTaskCommand => new(EditChecklistTask, DisplayError);
	private async Task EditChecklistTask(ChecklistTaskViewModel vm)
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			IsBusy = true;

			var viewModel = _map.From(vm).To<ChecklistTaskViewModel>();
			viewModel.ParentId = Id;

			viewModel.OnSaveRequested(SaveChecklistTaskAsync);
			viewModel.OnDeleteRequested(DeleteChecklistTaskAsync);

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

	private async Task SaveChecklistTaskAsync(ChecklistTaskViewModel viewModel)
	{
		try
		{
			var vm = Checklist.FirstOrDefault(i => i.Id == viewModel.Id);
			_map.From(viewModel).ToNullable(vm);

			await _navigationService.NavigateBack();
		}
		catch (Exception ex)
		{
			await _errorService.ShowAlertAsync("Error saving...", ex);
		}
	}

	public AsyncCommand<ChecklistTaskViewModel> DeleteChecklistTaskCommand => new(DeleteChecklistTaskAsync, DisplayError);
	private async Task<bool> DeleteChecklistTaskAsync(ChecklistTaskViewModel viewModel)
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
				var oldItem = Checklist.FirstOrDefault(i => i.Id == viewModel.Id);

				if (oldItem != null)
				{
					var index = Checklist.IndexOf(oldItem);

					if (index != -1)
					{
						Checklist.RemoveAt(index);
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
}