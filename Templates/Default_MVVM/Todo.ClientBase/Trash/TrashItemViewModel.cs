using System;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts.Trash;
using <# Project.Namespace#>.Contracts.Shared;

namespace <# Project.Namespace#>.ClientBase.Trash;

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
<#cs SetOutputPathAndSkipOtherDefinitions()#>