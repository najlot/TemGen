using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Contracts.ListItems;

public sealed class <#cs Write(Definition.Name);#>ListItem
{
	public Guid Id { get; set; }
<#cs
foreach(var entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	var typePrefix = entry.IsArray ? "List<" : "";
	var typeSuffix = entry.IsNullable ? "?" : "";
	typeSuffix = entry.IsArray ? ">" : typeSuffix;
	var suffix = entry.IsReference? "Id" : "";
	var def = "";
	if (entry.EntryType == "string")
	{
		def = " = string.Empty;";
	}
	WriteLine($"	public {typePrefix}{entry.EntryType}{typeSuffix} {entry.Field}{suffix} {{ get; set; }}{def}");
}

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>