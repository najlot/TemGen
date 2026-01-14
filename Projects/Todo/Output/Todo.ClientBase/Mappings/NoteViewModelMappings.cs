using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.Mappings;

[Mapping]
internal sealed partial class NoteViewModelMappings
{
	public static partial void MapToViewModel(IMap map, NoteUpdated from, NoteViewModel to);

	public static partial void MapToModel(IMap map, NoteViewModel from, NoteModel to);

	public static partial void MapToViewModel(IMap map, NoteModel from, NoteViewModel to);

}