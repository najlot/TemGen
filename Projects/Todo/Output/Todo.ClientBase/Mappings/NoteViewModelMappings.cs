using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.Mappings;

internal sealed class NoteViewModelMappings
{
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, NoteUpdated from, NoteViewModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;

	}

	public void MapToModel(IMap map, NoteViewModel from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, NoteModel from, NoteViewModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;

	}
}