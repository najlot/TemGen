using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Todo.Client.Data.Models;
using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModel;

public partial class TodoItemViewModel
{
	public ObservableCollection<ChecklistTaskViewModel> Checklist { get; set => Set(ref field, value); } = [];

	private void InitializeChecklistTracking()
	{
		foreach (var item in Checklist)
		{
			item.ChangeVisitor = _changeTracker;
		}
	}

	public RelayCommand AddChecklistTaskCommand => new(AddChecklistTask);
	private void AddChecklistTask()
	{
		var max = 0;

		if (Checklist.Count > 0)
		{
			max = Checklist.Max(e => e.Id) + 1;
		}

		var model = new ChecklistTaskModel() { Id = max };
		var viewModel = Map.From(model).To<ChecklistTaskViewModel>();
		viewModel.Id = max;
		viewModel.ParentId = Id;
		viewModel.ChangeVisitor = _changeTracker;

		Checklist.Add(viewModel);

		_changeTracker.Track(
			nameof(Checklist),
			() => Checklist.Remove(viewModel),
			() =>
			{
				viewModel.ChangeVisitor = _changeTracker;
				Checklist.Add(viewModel);
			});
	}

	public AsyncCommand<ChecklistTaskViewModel> DeleteChecklistTaskCommand => new(DeleteChecklistTaskAsync, t => HandleError(t.Exception));
	private async Task<bool> DeleteChecklistTaskAsync(ChecklistTaskViewModel? viewModel)
	{
		if (IsBusy || viewModel is null)
		{
			return false;
		}

		try
		{
			IsBusy = true;

			var oldItem = Checklist.FirstOrDefault(i => i.Id == viewModel.Id);

			if (oldItem is not null)
			{
				var index = Checklist.IndexOf(oldItem);

				if (index != -1)
				{
					Checklist.RemoveAt(index);

					_changeTracker.Track(
						nameof(Checklist),
						() =>
						{
							oldItem.ChangeVisitor = _changeTracker;
							Checklist.Insert(index, oldItem);
						},
						() => Checklist.Remove(oldItem));
				}
			}

			return true;
		}
		catch (Exception ex)
		{
			await NotificationService.ShowErrorAsync("Error deleting..." + ex.Message);
		}
		finally
		{
			IsBusy = false;
		}

		return false;
	}
}
