using System;
using Todo.Client.MVVM;
using Todo.Contracts.Notes;

namespace Todo.ClientBase.Notes;

public class NoteListItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }

	public bool IsFavorite
	{
		get;
		set => Set(ref field, value, () => RaisePropertyChanged(nameof(FavoriteIconKind)));
	}

	public string FavoriteIconKind => IsFavorite ? "Star" : "StarBorder";

	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;
}
