using Najlot.Map;
using Najlot.Map.Attributes;
using <#cs Write(Project.Namespace)#>.Client.Data.Models;
using <#cs Write(Project.Namespace)#>.ClientBase.ViewModel;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class <#cs Write(Definition.Name)#>ViewModelMappings
{
	public static partial void MapToModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>Model to);

	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static partial void MapFromViewModelToViewModel(IMap map, <#cs Write(Definition.Name)#>ViewModel from, <#cs Write(Definition.Name)#>ViewModel to);

	[MapValidateSource]
	public static partial void MapToViewModel(IMap map, <#cs Write(Definition.Name)#>Model from, <#cs Write(Definition.Name)#>ViewModel to);

	[MapValidateSource]
	public static partial void MapToViewModel(IMap map, <#cs Write(Definition.Name)#> from, <#cs Write(Definition.Name)#>ViewModel to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>