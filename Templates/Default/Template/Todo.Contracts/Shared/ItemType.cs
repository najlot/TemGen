namespace <# Project.Namespace#>.Contracts.Shared;

public enum ItemType
{
<#for definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name)
#>	<# definition.Name#>,
<#end#>}
<#cs SetOutputPathAndSkipOtherDefinitions()#>