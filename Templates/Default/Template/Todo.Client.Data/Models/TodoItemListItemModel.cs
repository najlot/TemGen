using System;
using System.Collections.Generic;
using <#cs Write(Project.Namespace)#>.Contracts;

namespace <#cs Write(Project.Namespace)#>.Client.Data.Models;

public class <#cs Write(Definition.Name)#>ListItemModel
{
	public Guid Id { get; set; }

<#cs
foreach(var entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2))
{
	if (entry.IsArray)
	{
		WriteLine($"	public List<{entry.EntryType}> {entry.Field} {{ get; set; }} = new();");
	}
	else
	{
		var q = entry.IsNullable? "?" : "";
		var suffix = entry.IsReference? "Id" : "";
		var defaultValue = entry.EntryType == "string" ? " = string.Empty;" : "";
		WriteLine($"	public {entry.EntryType}{q} {entry.Field}{suffix} {{ get; set; }}{defaultValue}");
	}
}
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>