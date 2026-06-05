using Najlot.Map;
using Najlot.Map.Attributes;
<#cs if (Definition.IsOwnedType || Definition.IsArray)
{
	var folderName = GetChildFeatureFolderName(Definition.Name);
	WriteLine($"using {Project.Namespace}.Client.Data.{folderName};");
	WriteLine($"using {Project.Namespace}.Contracts.{folderName};");
}
#>
namespace <# Project.Namespace#>.ClientBase.<#if
Definition.IsOwnedType || Definition.IsArray
#><#cs Write(GetChildFeatureFolderName(Definition.Name))#><#else#><# Definition.Name#>s<#end#>;

[Mapping]
internal sealed partial class <# Definition.Name#>ViewModelMappings
{
	public static partial void MapToModel(IMap map, <# Definition.Name#>ViewModel from, <# Definition.Name#>Model to);

	<#for entry in Entries.Where(e => e.IsArray)
#>[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
	<#end#>[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	public static partial void MapFromViewModelToViewModel(IMap map, <# Definition.Name#>ViewModel from, <# Definition.Name#>ViewModel to);

	<#for entry in Entries.Where(e => e.IsArray)
#>[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
	<#end#>private static partial void MapPartialToViewModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <# Definition.Name#>Model from, <# Definition.Name#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);<#if Entries.Any(e => e.IsArray)#>
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = [.. map.From<<# entry.EntryType#>Model>(from.<# entry.Field#>).To<<# entry.EntryType#>ViewModel>()];
<#end#>	}

	<#for entry in Entries.Where(e => e.IsArray)
#>[MapIgnoreProperty(nameof(to.<# entry.Field#>))]
	<#end#>private static partial void MapPartialToViewModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>ViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, <# Definition.Name#> from, <# Definition.Name#>ViewModel to)
	{
		MapPartialToViewModel(map, from, to);<#if Entries.Any(e => e.IsArray)#>
<#end#>
<#for entry in Entries.Where(e => e.IsArray)
#>		to.<# entry.Field#> = [.. map.From<<# entry.EntryType#>>(from.<# entry.Field#>).To<<# entry.EntryType#>ViewModel>()];
<#end#>	}
}<#cs SetOutputPath(!(Definition.IsOwnedType || Definition.IsArray));
if (Definition.IsOwnedType || Definition.IsArray)
{
	RelativePath = RelativePath.Replace("_ARRAY_OR_OWNED_", "");
	MoveChildToFeatureFolder();
}
#>