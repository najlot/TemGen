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
	if (entry.IsArray)
	{
		WriteLine($"	public List<{entry.EntryType}> {entry.Field} {{ get; set; }} = [];");
	}
	else
	{
		var q = entry.IsNullable? "?" : "";
		var entryType = entry.IsReference? entry.ReferenceType + "Model" : entry.EntryType;
		var defaultValue = "";
		if (!entry.IsNullable && entry.EntryType.ToLower() == "string")
		{
			defaultValue = " = string.Empty;";
		}
		WriteLine($"	public {entryType}{q} {entry.Field} {{ get; set; }}{defaultValue}");
	}
}

Result = Result.TrimEnd();
#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>