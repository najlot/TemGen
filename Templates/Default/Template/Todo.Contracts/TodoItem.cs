using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Contracts;

public <#cs Write(Definition.IsEnumeration ? "enum" : "class")#> <#cs Write(Definition.Name);#>
{
<#cs
if (Definition.IsEnumeration)
{
	foreach(var entry in Entries) WriteLine($"	{entry.Field},");
}
else
{
	if (Definition.IsArray)
	{
		WriteLine("	public int Id { get; set; }");
	}
	else
	{
		WriteLine("	public Guid Id { get; set; }");
	}
	
	foreach(var entry in Entries)
	{
		var typePrefix = entry.IsArray ? "List<" : "";
		var typeSuffix = entry.IsNullable ? "?" : "";
		typeSuffix = entry.IsArray ? ">" : typeSuffix;
		var suffix = entry.IsReference? "Id" : "";

		var defaultValue = "";
		if (entry.EntryType == "string")
		{
			defaultValue = " = string.Empty;";
		}
		else if (entry.IsArray)
		{
			defaultValue = " = [];";
		}

		WriteLine($"	public {typePrefix}{entry.EntryType}{typeSuffix} {entry.Field}{suffix} {{ get; set; }}{defaultValue}");
	}
}

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(false)#>