using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Notes;
using Todo.Contracts.Notes;

namespace Todo.ClientBase.Notes;

[Mapping]
internal sealed partial class NoteViewModelMappings
{
	private static partial void MapPartialToViewModel(IMap map, NoteUpdated from, NoteViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, NoteUpdated from, NoteViewModel to)
	{
		MapPartialToViewModel(map, from, to);
	}

	public static partial void MapToModel(IMap map, NoteViewModel from, NoteModel to);

	private static partial void MapPartialToViewModel(IMap map, NoteModel from, NoteViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, NoteModel from, NoteViewModel to)
	{
		MapPartialToViewModel(map, from, to);
	}

	public static partial void MapToViewModel(IMap map, NoteListItemModel from, NoteListItemViewModel to);

	public static partial void MapToViewModel(IMap map, NoteCreated from, NoteListItemViewModel to);

	public static partial void MapToViewModel(IMap map, NoteUpdated from, NoteListItemViewModel to);
}