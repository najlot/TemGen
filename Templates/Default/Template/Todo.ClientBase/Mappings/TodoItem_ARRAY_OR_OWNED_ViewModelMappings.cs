using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Mappings;

internal sealed class <#cs Write(Definition.Name)#>ViewModelMappings
{
	public static void MapToModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>Model to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("ViewModel", "Model")#>	}

	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapFromViewModelToViewModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		to.Id = from.Id;
		to.ParentId = from.ParentId;
<#cs WriteFromToMapping("Model", "ViewModel")#>	}

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("Model", "ViewModel")#>	}

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>ViewModel to)
	{
		to.Id = from.Id;
<#cs WriteFromToMapping("", "ViewModel")#>	}
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>