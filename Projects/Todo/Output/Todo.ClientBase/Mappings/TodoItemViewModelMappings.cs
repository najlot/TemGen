using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.Mappings;

internal sealed class TodoItemViewModelMappings
{
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, TodoItemUpdated from, TodoItemViewModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = [.. map.From<ChecklistTask>(from.Checklist).To<ChecklistTaskViewModel>()];

		foreach (var e in to.Checklist) e.ParentId = from.Id;
	}

	public void MapToModel(IMap map, TodoItemViewModel from, TodoItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = map.From<ChecklistTaskViewModel>(from.Checklist).ToList<ChecklistTaskModel>();
	}

	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, TodoItemModel from, TodoItemViewModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = [.. map.From<ChecklistTaskModel>(from.Checklist).To<ChecklistTaskViewModel>()];

		foreach (var e in to.Checklist) e.ParentId = from.Id;
	}
}