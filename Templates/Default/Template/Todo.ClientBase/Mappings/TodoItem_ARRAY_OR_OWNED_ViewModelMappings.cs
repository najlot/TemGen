using Najlot.Map;
using Najlot.Map.Attributes;
using <# Project.Namespace#>.Client.Data.Models;
using <# Project.Namespace#>.ClientBase.ViewModel;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.ClientBase.Mappings;

[Mapping]
internal sealed partial class <# Definition.Name#>ViewModelMappings
{
	public static partial void MapToModel(IMap map, <# Definition.Name#>ViewModel from, <# Definition.Name#>Model to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	public static partial void MapFromViewModelToViewModel(IMap map, <# Definition.Name#>ViewModel from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static partial void MapToViewModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static partial void MapToViewModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>ViewModel to);
}<#cs
SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
#>