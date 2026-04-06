using System;
using Todo.Client.MVVM;
using Todo.Contracts;

namespace Todo.ClientBase.ViewModels;

public sealed class TrashItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }
	public ItemType Type { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;
	public DateTime? DeletedAt { get; set => Set(ref field, value); }

	public string TypeDisplay => Type.ToString();
	public string DeletedAtDisplay => DeletedAt?.ToLocalTime().ToString("g") ?? string.Empty;
}
