using System;
using System.Collections.Generic;
using <# Project.Namespace#>.Contracts;

namespace <# Project.Namespace#>.Client.Data.Models;

public class <# Definition.Name#>ListItemModel
{
	public Guid Id { get; set; }

<#for entry in Entries.Where(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Take(2)
#><#if entry.IsArray
#>	public List<<# entry.EntryType#>> <# entry.Field#> { get; set; } = new();
<#else
#>	public <# entry.EntryType#><#cs Write(entry.IsNullable ? "?" : "")#> <# entry.Field#><#cs Write(entry.IsReference ? "Id" : "")#> { get; set; }<#if entry.EntryType == "string"
#> = string.Empty;<#end#>
<#end#><#end
#>}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>