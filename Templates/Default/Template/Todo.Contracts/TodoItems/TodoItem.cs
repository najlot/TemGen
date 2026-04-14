using System;
using System.Collections.Generic;
<#if !Definition.IsEnumeration
	&& (NeedsSharedEnumerationChildren() || NeedsSharedOwnedChildren() || NeedsSharedArrayChildren())
#>using <# Project.Namespace#>.Contracts.Shared;
<#end#>
namespace <# Project.Namespace#>.Contracts.<#if
Definition.IsOwnedType || Definition.IsArray || Definition.IsEnumeration
#><#cs Write(GetChildFeatureFolderName(Definition.Name))#><#else#><# Definition.Name#>s<#end#>;

public <#cs Write(Definition.IsEnumeration ? "enum" : "class")#> <# Definition.Name#>
{
<#if Definition.IsEnumeration
#><#for entry in Entries
#>	<# entry.Field#>,
<#end#><#else
#>	public <#cs Write(Definition.IsArray ? "int" : "Guid")#> Id { get; set; }
<#for entry in Entries
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#elseif entry.IsArray
#> = [];<#elseif entry.IsOwnedType && !entry.IsNullable
#> = new();<#end#>
<#end#><#end
#>
}<#cs SetOutputPath(false);
if (Definition.IsOwnedType || Definition.IsArray || Definition.IsEnumeration)
{
	MoveChildToFeatureFolder();
}
#>