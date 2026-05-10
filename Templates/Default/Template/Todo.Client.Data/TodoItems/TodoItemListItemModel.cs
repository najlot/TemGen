using System;
using System.Collections.Generic;
using <# Project.Namespace#>.Client.Localisation;
using <# Project.Namespace#>.Contracts.<# Definition.Name#>s;
<#if NeedsSharedEnumerationChildren()
#>using <# Project.Namespace#>.Contracts.Shared;
<#end#>
namespace <# Project.Namespace#>.Client.Data.<# Definition.Name#>s;

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
#>
	public string DisplayText => Id == Guid.Empty
		? CommonLoc.NothingSelected
<#if Entries.Any(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType))
#>		: Convert.ToString(<#cs Write(Entries.First(e => !(e.IsKey || e.IsArray || e.IsReference || e.IsOwnedType)).Field)#>) ?? CommonLoc.Untitled;
<#else
#>		: Id.ToString();
<#end#>
}<#cs SetOutputPath(Definition.IsOwnedType || Definition.IsEnumeration || Definition.IsArray)#>