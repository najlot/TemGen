using System;
using System.Collections.Generic;
<#cs#>
namespace <# Project.Namespace#>.Contracts.<# Definition.Name#>s;

public sealed class <# Definition.Name#>ListItem
{
	public Guid Id { get; set; }
<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2)
#>	public <#cs Write(entry.IsArray ? "List<" : "")#><# entry.EntryType#><#cs Write(entry.IsArray ? ">" : entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#end#>
<#end
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>