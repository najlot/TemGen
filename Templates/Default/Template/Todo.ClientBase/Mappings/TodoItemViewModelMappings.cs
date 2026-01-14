using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using <#cs Write(Project.Namespace)#>.Contracts;
using <#cs Write(Project.Namespace)#>.Contracts.Events;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Mappings;

internal sealed class <#cs Write(Definition.Name)#>ViewModelMappings
{
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Updated from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "ViewModel", int.MaxValue, MapArrayStrategy.RemapToCustomCollection)#>
<#cs foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
}#>	}

	public static void MapToModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("ViewModel", "Model")#>	}

	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("Model", "ViewModel", int.MaxValue, MapArrayStrategy.RemapToCustomCollection)#>
<#cs foreach(var entry in Entries.Where(e => e.IsArray))
{
	WriteLine($"		foreach (var e in to.{entry.Field}) e.ParentId = from.Id;");
}#>	}
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>