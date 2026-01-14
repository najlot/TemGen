using Najlot.Map;
using Najlot.Map.Attributes;
using System;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Service.Model;

namespace Todo.Service.Mappings;

[Mapping]
internal partial class NoteMappings
{
	public static NoteCreated MapToCreated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static NoteUpdated MapToUpdated(IMap map, NoteModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.Color);

	public static partial void MapToModel(IMap map, CreateNote from, NoteModel to);

	public static void MapToModel(IMap map, UpdateNote from, NoteModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.Color = from.Color;
	}

	public static partial void MapToModel(IMap map, NoteModel from, Note to);

	public static Expression<Func<NoteModel, NoteListItem>> GetListItemExpression()
	{
		return from => new NoteListItem
		{
			Id = from.Id,
			Title = from.Title,
			Content = from.Content
		};
	}

	public static void MapToModel(IMap map, NoteModel from, NoteListItem to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}
}