using MongoDB.Bson.Serialization.Attributes;
using <#cs Write(Project.Namespace)#>.Contracts;
using System;
using System.Collections.Generic;

namespace <#cs Write(Project.Namespace)#>.Service.Model;

[BsonIgnoreExtraElements]
public class <#cs Write(Definition.Name)#>Model
{
	[BsonId]
	public Guid Id { get; set; }
<#cs
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

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>