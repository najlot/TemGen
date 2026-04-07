using System;
using Todo.Client.MVVM;

namespace Todo.ClientBase.ViewModels;

public class TodoItemListItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }

	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;
}
