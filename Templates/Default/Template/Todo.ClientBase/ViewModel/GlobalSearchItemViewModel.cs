using System;
using <# Project.Namespace#>.Client.MVVM;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.ClientBase.ViewModel;

public sealed class GlobalSearchItemViewModel : AbstractViewModel
{
	public Guid Id { get; set => Set(ref field, value); }
	public ItemType Type { get; set => Set(ref field, value); }
	public string Title { get; set => Set(ref field, value); } = string.Empty;
	public string Content { get; set => Set(ref field, value); } = string.Empty;

	public string TypeDisplay => Type.ToString();
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>