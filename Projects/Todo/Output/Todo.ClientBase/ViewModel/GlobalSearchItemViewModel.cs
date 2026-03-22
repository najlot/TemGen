using System;
using Todo.Client.MVVM;
using Todo.Contracts;

namespace Todo.ClientBase.ViewModel;

public sealed class GlobalSearchItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }
	public ItemType Type { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;

	public string TypeDisplay => Type.ToString();
}
