using MongoDB.Bson.Serialization.Attributes;
using <# Project.Namespace#>.Contracts;
using System;
using System.Collections.Generic;

namespace <# Project.Namespace#>.Service.Model;

[BsonIgnoreExtraElements]
public class <# Definition.Name#>Model
{
	[BsonId]
	public Guid Id { get; set; }
	public DateTime? DeletedAt { get; set; }
<#for entry in Entries
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#elseif entry.IsArray
#> = [];<#end#>
<#end#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>