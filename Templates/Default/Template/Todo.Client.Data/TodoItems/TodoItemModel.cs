using System;
using System.Collections.Generic;
using <# Project.Namespace#>.Contracts.<#if
Definition.IsOwnedType || Definition.IsArray
#><#cs Write(GetChildFeatureFolderName(Definition.Name))#><#else#><# Definition.Name#>s<#end#>;

namespace <# Project.Namespace#>.Client.Data.<#if
Definition.IsOwnedType || Definition.IsArray
#><#cs Write(GetChildFeatureFolderName(Definition.Name))#><#else#><# Definition.Name#>s<#end#>;

public class <# Definition.Name#>Model
{
	public <#cs Write(Definition.IsArray ? "int" : "Guid")#> Id { get; set; }

<#for entry in Entries
#><#if entry.IsArray
#>	public List<<# entry.EntryType#>Model> <# entry.Field#> { get; set; } = [];
<#else
#>	public <# entry.EntryType#><#if entry.IsOwnedType
#>Model<#end#><#cs Write(entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#elseif entry.IsOwnedType && !entry.IsNullable
#> = new();<#end#>
<#end#><#end
#>}<#cs SetOutputPath(Definition.IsEnumeration);
if (!Definition.IsEnumeration && (Definition.IsOwnedType || Definition.IsArray))
{
	MoveChildToFeatureFolder();
}
#>